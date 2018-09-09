import numpy as np
import matplotlib.pyplot as plt
import FlightData
import os
import CustomFigure


def find_nearest_indice(array, value):
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return idx


def transform(trials, times, sample_rate, channels, cycles, frequencies):
    number_of_measurements = trials.shape[0]
    number_of_channels = trials.shape[1]
    number_of_trials = trials.shape[2]

    # Baseline window used during db normalization
    baseline_window = [-500, -200]

    # Time for wavelet, should contain odd number of samples
    time = np.arange(-2, 2, 1 / sample_rate)
    if time[-1] != 2:
        time = np.append(time, 2)

    # Calculate half of wavelet, will be used to trim result of convolution
    half_wave = int((len(time) - 1) / 2)

    # Calculate length of convolution
    convolution_length = len(time) + (number_of_measurements * number_of_trials) - 1
    coefficients = np.zeros((len(channels),len(cycles), len(frequencies), number_of_measurements))
    for channel_index in range(0, len(channels)):
        channel_name = channels[channel_index]
        print(f"Performing Morlet Wavelet Transform on channel {channel_name}")

        # Prepare coefficients array that will contain result of convolution of channels with Morlet wavelet
        channel_coefficients = np.zeros((len(cycles), len(frequencies), number_of_measurements))

        # Calculate indices for baseline window
        baseline_indices = np.zeros(2)
        baseline_indices[0] = find_nearest_indice(times, baseline_window[0])
        baseline_indices[1] = find_nearest_indice(times, baseline_window[1])

        # Calculate FFT for given channel
        channel_fft = np.fft.fft(np.reshape(trials[:, channel_index, :].transpose(), (-1)), convolution_length)

        # Loop over cycles
        for cycle_index in range(0, len(cycles)):
            for frequency_index in range(0, len(frequencies)):
                # Calculate standard deviation for gaussian function
                s = cycles[cycle_index] / (2 * np.pi * frequencies[frequency_index])

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

                # Reshape 1D array to 2D in order to split trials
                convolvedSignal = np.reshape(convolvedSignal, (number_of_measurements, number_of_trials), order="F")

                # Calculate power
                channel_coefficients[cycle_index, frequency_index, :] = np.mean(np.abs(convolvedSignal) ** 2, 1)

            # Perform db normalization
            baseline_mean = np.mean(
                channel_coefficients[cycle_index, :, int(baseline_indices[0]):int(baseline_indices[1])], 1)
            coefficients[channel_index, cycle_index, :, :] = 10 * np.log10(
                np.divide(channel_coefficients[cycle_index, :, :], baseline_mean[:, np.newaxis]))

    return coefficients
