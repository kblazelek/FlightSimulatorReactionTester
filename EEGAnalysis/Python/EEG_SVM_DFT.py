import numpy as np
import matplotlib.pyplot as plt
import FlightData
import os
import itertools
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score
from sklearn import svm
import csv

# This script uses SVM classifier to distinguish between left and right arrow in EEG response.
# It uses power spectrum as features
dataDir = './Data/'
if not os.path.exists(dataDir):
    os.makedirs(dataDir)

all_channel_names = ['AF3', 'F7', 'F3', 'FC5', 'T7', 'P7', 'O1', 'O2', 'P8', 'FC6', 'F8', 'AF4']
resultsFile = r'./Data/SVM_DFT_Results.csv'
flight_numbers = [1, 2, 3]
features_list = []
label_list = []
fft_start = 0
fft_stop = 35  # index where frequency is equal to 30 after DFT
samples_before = 0  # How many samples to take before the event
samples_after = 150  # How many samples to take after the event
# for channel_names in itertools.combinations(all_channel_names, len(all_channel_names)): use features from all channels
# for channel_names in itertools.combinations(all_channel_names, 1): use features from 1 channel
# for channel_names in itertools.combinations(all_channel_names, 2): use features from pairs of channels
for channel_names in itertools.combinations(all_channel_names, len(all_channel_names)):
    features_list = []
    label_list = []
    for flightNumber in flight_numbers:
        print(f"Gathering left and right trials for flight {flightNumber}")
        (l_trials, l_times, l_sample_rate, l_channels) = FlightData.get_trials_for_arrow(flightNumber, 'Left',
                                                                                         samples_before, samples_after)
        (r_trials, r_times, r_sample_rate, r_channels) = FlightData.get_trials_for_arrow(flightNumber, 'Right',
                                                                                         samples_before, samples_after)
        for i in range(0, l_trials.shape[2]):
            features = []
            for channel_name in channel_names:
                channel_index = l_channels.index(channel_name)
                coefficients = np.abs(np.fft.fft(l_trials[:, channel_index, i]))[fft_start:fft_stop]
                for f in coefficients:
                    features.append(f)
            features_list.append(features)
            label_list.append('Left')

        for i in range(0, r_trials.shape[2]):
            features = []
            for channel_name in channel_names:
                channel_index = r_channels.index(channel_name)
                coefficients = np.abs(np.fft.fft(r_trials[:, channel_index, i]))[fft_start:fft_stop]
                for f in coefficients:
                    features.append(f)
            features_list.append(features)
            label_list.append('Right')

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
        fields = [channel_names, flight_numbers, samples_before, samples_after, fft_start, fft_stop, title, accuracy]
        with open(resultsFile, 'a', ) as f:
            writer = csv.writer(f, lineterminator='\n', delimiter=";")
            writer.writerow(fields)
