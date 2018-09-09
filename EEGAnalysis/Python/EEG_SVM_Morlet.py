import numpy as np
import matplotlib.pyplot as plt
import FlightData
import os
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score
from sklearn import svm
import csv
import itertools

# This script uses SVM classifier to distinguish between left and right arrow in EEG response.
# It uses output from morlet wavelet transform as features
dataDir = './Data/'
if not os.path.exists(dataDir):
    os.makedirs(dataDir)

def find_nearest_indice(array, value):
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return idx


def transform(trial, times, sample_rate, frequencies):
    number_of_measurements = len(trial)

    # Time for wavelet, should contain odd number of samples
    time = np.arange(-2, 2, 1 / sample_rate)
    if time[-1] != 2:
        time = np.append(time, 2)

    # Calculate half of wavelet, will be used to trim result of convolution
    half_wave = int((len(time) - 1) / 2)

    # Calculate length of convolution
    convolution_length = len(time) + number_of_measurements - 1

    # Prepare coefficients array that will contain result of convolution of channels with Morlet wavelet
    coefficients = np.zeros((len(frequencies), number_of_measurements))

    # Calculate FFT for given channel
    channel_fft = np.fft.fft(np.reshape(trial.transpose(), (-1)), convolution_length)

    for frequency_index in range(0, len(frequencies)):
        # Calculate standard deviation for gaussian function
        s = 6 / (2 * np.pi * frequencies[frequency_index])

        # Create gaussian function
        gaussian = np.exp(-time ** 2 / (2 * s ** 2))

        # Create sinus function
        sinus = np.exp(2 * 1j * np.pi * frequencies[frequency_index] * time)

        # Create complex Morlet wavelet by multiplying sinus and gaussian point by point
        morlet = sinus * gaussian

        # Take FFT of morlet wavelet and pad zeroes
        morlet_FFT = np.fft.fft(morlet, convolution_length)

        # Normalize FFT so that the biggest value is 1
        morlet_FFT = morlet_FFT / max(morlet_FFT)

        # run convolution, trim edges, and reshape to 2D (time X trials)
        convolvedSignal = np.fft.ifft(morlet_FFT * channel_fft)

        # Convolution results in output being longer that the original signal (output = N + M - 1)
        # Trim to the size of signal
        convolvedSignal = convolvedSignal[(half_wave):(-half_wave)]

        # Calculate power
        coefficients[frequency_index, :] = np.abs(convolvedSignal) ** 2

    return coefficients


def extract_features(coefficients, times):
    features = coefficients.ravel()
    return features


all_channel_names = ['AF3', 'F7', 'F3', 'FC5', 'T7', 'P7', 'O1', 'O2', 'P8', 'FC6', 'F8', 'AF4']
resultsFile = r'./Data/SVM_WholeMorletPerChannelCombinedChannels_Results.csv'
# Morlet wavelet frequencies
frequencies = np.linspace(2, 30, 40)
flight_numbers = [1, 2, 3]
samples_before = 0
samples_after = 150
features_list = []
label_list = []
data_left = []
data_right = []

# Load all EEG data into memory
for flightNumber in flight_numbers:
    print(f"Reading left and right arrow data into memory from flight number {flightNumber}")
    (l_trials, l_times, l_sample_rate, l_channels) = FlightData.get_trials_for_arrow(flightNumber, 'Left',
                                                                                     samples_before, samples_after)
    data_left.append((l_trials, l_times, l_sample_rate, l_channels))
    (r_trials, r_times, r_sample_rate, r_channels) = FlightData.get_trials_for_arrow(flightNumber, 'Right',
                                                                                     samples_before, samples_after)
    data_right.append((r_trials, r_times, r_sample_rate, r_channels))

# for channel_names in itertools.combinations(all_channel_names, len(all_channel_names)): use features from all channels
# for channel_names in itertools.combinations(all_channel_names, 1): use features from 1 channel
# for channel_names in itertools.combinations(all_channel_names, 2): use features from pairs of channels
for channel_names in itertools.combinations(all_channel_names, len(all_channel_names)):
    features_list = []
    label_list = []
    for i in range(0, len(data_right)):
        print(f"Extracting features for flight number {i+1}")
        (l_trials, l_times, l_sample_rate, l_channels) = data_left[i]
        (r_trials, r_times, r_sample_rate, r_channels) = data_right[i]

        for i in range(0, l_trials.shape[2]):
            features = []
            for channel_name in channel_names:
                channel_index = l_channels.index(channel_name)
                coefficients = transform(l_trials[:, channel_index, i], l_times, l_sample_rate, frequencies)
                for f in extract_features(coefficients, l_times):
                    features.append(f)
            features_list.append(features)
            label_list.append('Left')

        for i in range(0, r_trials.shape[2]):
            features = []
            for channel_name in channel_names:
                channel_index = r_channels.index(channel_name)
                coefficients = transform(r_trials[:, channel_index, i], r_times, r_sample_rate, frequencies)
                for f in extract_features(coefficients, r_times):
                    features.append(f)
            features_list.append(features)
            label_list.append('Right')

    # normalizacja
    C = 1.0  # SVM regularization parameter
    models = (svm.SVC(kernel='linear', C=C),
              svm.LinearSVC(C=C),
              svm.SVC(kernel='rbf', gamma=0.7, C=C),
              svm.SVC(kernel='poly', degree=3, C=C))

    # title for the plots
    titles = ('SVC with linear kernel',
              'LinearSVC (linear kernel)',
              'SVC with RBF kernel (gamma 0.7)',
              'SVC with polynomial (degree 3) kernel')

    # Split train and test data
    X_train, X_test, y_train, y_test = train_test_split(features_list, label_list, test_size=0.33, random_state=42)

    # Train models
    models = (clf.fit(X_train, y_train) for clf in models)
    for clf, title in zip(models, titles):
        y_pred = clf.predict(X_test)
        accuracy = accuracy_score(y_test, y_pred)
        print(f"Accuracy of {title}: {accuracy}")
        fields = [channel_names, flight_numbers, samples_before, samples_after, title, accuracy]
        with open(resultsFile, 'a', ) as f:
            writer = csv.writer(f, lineterminator='\n', delimiter=";")
            writer.writerow(fields)
