import pywt
import numpy as np
import matplotlib.pyplot as plt
import scipy.fftpack
from pylab import *
from pandas import read_csv, DataFrame
from ArrowStateParser import *


def extract_trials(eeg_with_arrows_data_frame, samples_before_event, samples_after_event, channels):
    """
    Extracts trials from array (EEG data + arrow states) into 3d matrix of EEG data per trial for selected channels.
    I.e. We have 10 rows of EEG data + arrow states, 14 channels and arrow state was equal to 2 three times.
    In other words we had three events during this time (user clicked correct arrow 3 times).
    Let's say we want 1 sample before and 1 sample after an event (so that's 3 samples per trial).
    The output will be of size (trials * 3) x (count of channels) x (trials).
    trials[:,:,0] will contain EEG data for first trial
    :param eeg_with_arrows_data_frame: EEG data + arrow states (DataFrame). Event is value 2 in column ArrowState.
    :param samples_before_event: How many samples before an event to extract for each trial
    :param samples_after_event: How many samples after an event to extract for each trial
    :param channels: Which channels to extract. Allowed values: AF3;F7;F3;FC5;T7;P7;O1;O2;P8;T8;FC6;F4;F8;AF4
    :return 3d matrix of eeg data per trial (measurements x channels x trials)
    """
    # Header should look like follows
    # AF3;F7;F3;FC5;T7;P7;O1;O2;P8;T8;FC6;F4;F8;AF4;ArrowState;Sampling Rate;
    event_row_numbers = eeg_with_arrows_data_frame.loc[eeg_with_arrows_data_frame['ArrowState'] == 2].axes[
        0]  # returns row numbers when event happened
    total_samples_per_trial = samples_before_event + 1 + samples_after_event
    trials = np.zeros((total_samples_per_trial, len(channels), event_row_numbers.size))
    for i in range(0, event_row_numbers.size):
        event_row = event_row_numbers[i]  # Gets row number where arrow was clicked (event)
        row_range = slice(event_row - samples_before_event, event_row + samples_after_event)
        trials[:, :, i] = eeg_with_arrows_data_frame.loc[row_range, channels]
    return trials


def extract_trials_with_arrow_shown_in_the_middle(eeg_with_arrows_data_frame, samples_before_event, samples_after_event,
                                                  channels):
    """
    Returns trials with arrow shown in the middle
    :param eeg_with_arrows_data_frame: EEG data + arrow states (DataFrame). Event is value 2 in column ArrowState.
    :param samples_before_event: How many samples before an event to extract for each trial
    :param samples_after_event: How many samples after an event to extract for each trial
    :param channels: Which channels to extract. Allowed values: AF3;F7;F3;FC5;T7;P7;O1;O2;P8;T8;FC6;F4;F8;AF4
    :return:  3d matrix of eeg data per trial (measurements x channels x trials)
    """
    arrowStates = eeg_with_arrows_data_frame.values[:, 14]
    event_row_numbers = ArrowStateParser().get_indexes_when_arrow_appeared_on_screen(arrowStates)
    total_samples_per_trial = samples_before_event + 1 + samples_after_event
    trials = np.zeros((total_samples_per_trial, len(channels), event_row_numbers.size))
    for i in range(0, event_row_numbers.size):
        event_row = event_row_numbers[i]  # Gets row number where arrow was clicked (event)
        row_range = slice(event_row - samples_before_event, event_row + samples_after_event)
        trials[:, :, i] = eeg_with_arrows_data_frame.loc[row_range, channels]
    return trials
