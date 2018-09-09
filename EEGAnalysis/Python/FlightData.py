import numpy as np
from pandas import read_csv, DataFrame
import TrialExtractor as te
import FutureEventResultParser


def get_trials_to_skip(flight_number):
    """
    Returns trials that contained artifacts in given flight
    :param flight_number:
    :return: indexes of trials with artifacts
    """
    if flight_number == 1:
        return [0, 1, 2, 4, 8, 22, 24, 25, 26, 28, 29, 38, 43, 44, 45, 51]
    elif flight_number == 2:
        return [13, 14, 25, 36, 37, 38, 51]
    elif flight_number == 3:
        return [0, 1, 3, 4, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24, 25, 26, 29, 36, 37,
                38, 39, 42, 45, 46, 55, 56]
    else:
        raise ValueError(f"There was no flight with number {flight_number}")


def get_trials(flight_number, samples_before_event, samples_after_event, remove_artifacts=True):
    """
    Returns trials for given flight
    :param flight_number:
    :param samples_before_event: How many samples to take before an event
    :param samples_after_event: How many samples to take after an event
    :param remove_artifacts: Do you want to remove trials and channels with artifacts?
    :return: trials, times, sample_rate, channels
    """
    channels = ['AF3', 'F7', 'F3', 'FC5', 'T7', 'P7', 'O1', 'O2', 'P8', 'T8', 'FC6', 'F4', 'F8', 'AF4']
    channels_to_skip = []
    trials_to_skip = get_trials_to_skip(flight_number)
    if flight_number == 1:
        data = read_csv('./../../../Data/Flight1_EEG.csv', sep=";")
        sample_rate = 128.0
        channels_to_skip = ['T8']
    elif flight_number == 2:
        data = read_csv('./../../../Data/Flight2_EEG.csv', sep=";")
        sample_rate = 128.0
        channels_to_skip = ['T8']
    elif flight_number == 3:
        data = read_csv('./../../../Data/Flight3_EEG.csv', sep=";")
        sample_rate = 128.0
        channels_to_skip = ['F4']
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


def get_trials_for_arrow(flight_number, arrow_name, samples_before_event, samples_after_event, remove_artifacts=True):
    """
    Returns trials for given flight and requested arrow
    :param flight_number:
    :param arrow_name: Name of arrow to return trials for (Left, Right, Up, Down)
    :param samples_before_event: How many samples to take before an event
    :param samples_after_event: How many samples to take after an event
    :param remove_artifacts: Do you want to remove trials and channels with artifacts?
    :return: trials, times, sample_rate, channels
    """
    trials, times, sample_rate, channels = get_trials(flight_number, samples_before_event, samples_after_event,
                                                      remove_artifacts)
    reaction_times, arrows, delays = get_future_event_set_result(flight_number, remove_artifacts)
    arrows_data_frame = DataFrame(arrows)

    # Find indexes where requested arrow appeared
    indexes_to_keep = arrows_data_frame.loc[arrows_data_frame[0] == arrow_name].index

    # Filter trials so that only trials for requested arrow remain
    trials = trials[:, :, indexes_to_keep]

    return trials, times, sample_rate, channels


def get_future_event_set_result(flight_number, remove_data_from_faulty_trials=True):
    """
    Returns information from XML file for given flight about reactions to stimulus
    :param flight_number:
    :param remove_data_from_faulty_trials: Do you want to remove responses, that were part of trials with artifacts?
    :return: reaction_times, arrows, delays
    """
    if flight_number == 1:
        reaction_times, arrows, delays = FutureEventResultParser.parse('./../../../Data/Flight1_ReactionTimes.xml')
    elif flight_number == 2:
        reaction_times, arrows, delays = FutureEventResultParser.parse('./../../../Data/Flight2_ReactionTimes.xml')
    elif flight_number == 3:
        reaction_times, arrows, delays = FutureEventResultParser.parse('./../../../Data/Flight3_ReactionTimes.xml')
    else:
        raise ValueError(f"There was no flight with number {flight_number}")

    if remove_data_from_faulty_trials:
        # Get indexes of trials containing artifacts
        trials_to_skip = get_trials_to_skip(flight_number)

        # Get all indexes in array
        indexes_to_keep = np.arange(len(reaction_times))

        # Remove indexes that we want to skip
        indexes_to_keep = [i for i in indexes_to_keep if i not in trials_to_skip]

        # Filter arrays so that only data for artifact-free trials remain
        reaction_times = reaction_times[indexes_to_keep]
        arrows = arrows[indexes_to_keep]
        delays = delays[indexes_to_keep]

    return reaction_times, arrows, delays


def get_future_event_set_result_for_arrow(flight_number, arrow_name, remove_data_from_faulty_trials=True):
    """
    Returns information from XML file for given flight and arrow name about reactions to stimulus
    :param flight_number:
    :param arrow_name: Name of arrow to return trials for (Left, Right, Up, Down)
    :param remove_data_from_faulty_trials: Do you want to remove responses, that were part of trials with artifacts?
    :return: reaction_times, arrows, delays
    """
    reaction_times, arrows, delays = get_future_event_set_result(flight_number, remove_data_from_faulty_trials)
    arrows_data_frame = DataFrame(arrows)

    # Find indexes where requested arrow appeared
    indexes_to_keep = arrows_data_frame.loc[arrows_data_frame[0] == arrow_name].index

    # Filter arrays so that only data for requested arrow remain
    reaction_times = reaction_times[indexes_to_keep]
    arrows = arrows[indexes_to_keep]
    delays = delays[indexes_to_keep]

    return reaction_times, arrows, delays
