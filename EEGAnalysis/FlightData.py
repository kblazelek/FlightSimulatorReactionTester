import numpy as np
from pandas import read_csv, DataFrame
import TrialExtractor as te
import FutureEventResultParser

def get_trials(flight_number, samples_before_event, samples_after_event, remove_artifacts=True):
    channels = ['AF3', 'F7', 'F3', 'FC5', 'T7', 'P7', 'O1', 'O2', 'P8', 'T8', 'FC6', 'F4', 'F8', 'AF4']
    channels_to_skip = []
    trials_to_skip = []
    if flight_number == 1:
        data = read_csv('./Data/2018.17.8_14.31.16_EEG_With_Header.csv', sep=";")
        sample_rate = 128.0
        channels_to_skip = ['T8']
        # 22 and 51 was detected using butterfly plot
        # 0, 1, 2, 4, 8, 24, 25, 26, 28, 29, 38, 43, 44, 45 was detected using ERP plot
        trials_to_skip = [0, 1, 2, 4, 8, 22, 24, 25, 26, 28, 29, 38, 43, 44, 45, 51]
    elif flight_number == 2:
        data = read_csv('./Data/2018.08.20_13.59.46_EEG.csv', sep=";")
        sample_rate = 128.0
        channels_to_skip = ['T8']
        # 51 was detected using butterfly plot
        # 13, 14, 25, 36, 37, 38 was detected using ERP plot
        trials_to_skip = [13, 14, 25, 36, 37, 38, 51]
    elif flight_number == 3:
        data = read_csv('./Data/2018.08.30_13.51.46_EEG.csv', sep=";")
        sample_rate = 128.0
        channels_to_skip = ['F4']
        # all were detected by ERP plot
        trials_to_skip = [0, 1, 3, 4, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24, 25, 26, 29, 36, 37,
                          38, 39, 42, 45, 46, 55, 56]
    elif flight_number == 4:
        data = read_csv('./Data/Example.csv', sep=";")
        sample_rate = 256.0
    else:
        raise ValueError(f"There was no flight with number {flight_number}")

    data = DataFrame(data)

    # Remove DC offset
    for i in range(0, len(channels)):
        data.iloc[:, i] = data.iloc[:, i] - np.mean(data.iloc[:, i])

    # Remove channels containing artifacts
    if remove_artifacts and channels_to_skip:
        channels = [c for c in channels if c not in channels_to_skip]

    # Get trial data
    trials = te.extract_trials_with_arrow_shown_in_the_middle(data, samples_before_event, samples_after_event, channels)

    # Remove trials containing artifacts
    if remove_artifacts and trials_to_skip:
        trial_indexes = np.arange(trials.shape[2])
        trial_indexes = [t for t in trial_indexes if t not in trials_to_skip]
        trials = trials[:, :, trial_indexes]

    times = np.linspace(-samples_before_event / sample_rate * 1000, samples_after_event / sample_rate * 1000,
                        trials.shape[0])

    return trials, times, sample_rate, channels

def get_future_event_set_result(flight_number):
    if flight_number == 1:
        reactionTimes, arrows, delays = FutureEventResultParser.parse('./Data/2018.17.8_15.32.00_ReactionTimes.xml')
    elif flight_number == 2:
        reactionTimes, arrows, delays = FutureEventResultParser.parse('./Data/2018.08.20_15.00.21_ReactionTimes.xml')
    elif flight_number == 3:
        reactionTimes, arrows, delays = FutureEventResultParser.parse('./Data/2018.08.30_14.52.28_ReactionTimes.xml')

    return reactionTimes, arrows, delays