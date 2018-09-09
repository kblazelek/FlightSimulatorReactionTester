import numpy as np
import FlightData
import os
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score
from sklearn import svm
import csv
import itertools

# This script uses SVM classifier to distinguish between left and right arrow in EEG response.
# It uses trial amplitude variations as features
dataDir = './Data/'
if not os.path.exists(dataDir):
    os.makedirs(dataDir)

def find_nearest_indice(array, value):
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return idx


def extract_features(coefficients, times):
    features = np.sqrt(np.var(coefficients))
    return features


all_channel_names = ['AF3', 'F7', 'F3', 'FC5', 'T7', 'P7', 'O1', 'O2', 'P8', 'FC6', 'F8', 'AF4']
resultsFile = r'./Data/SVM_Variance_Results.csv'
flight_numbers = [1, 2, 3]
samples_before = 0
samples_after = 150
features_list = []
label_list = []
data_left = []
data_right = []
max = 0
max_acc_channel = ''
# Load EEG data into memory
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
                features.append(extract_features(l_trials[:, channel_index, i], l_times))
            features_list.append(features)
            label_list.append('Left')

        for i in range(0, r_trials.shape[2]):
            features = []
            for channel_name in channel_names:
                channel_index = r_channels.index(channel_name)
                features.append(extract_features(r_trials[:, channel_index, i], r_times))
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
        if accuracy > max:
            max = accuracy
            max_acc_channel = channel_name
        print(f"Accuracy of {title}: {accuracy}")
        fields = [channel_names, flight_numbers, samples_before, samples_after, title, accuracy]
        with open(resultsFile, 'a', ) as f:
            writer = csv.writer(f, lineterminator='\n', delimiter=";")
            writer.writerow(fields)

print(f"Max accuracy: {max} (channel {max_acc_channel})")
