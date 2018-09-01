import numpy as np
import matplotlib.pyplot as plt
import FlightData
import os
import CustomFigure


def find_nearest_indice(array, value):
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return idx


wtDir = './Images/WT'
if not os.path.exists(wtDir):
    os.makedirs(wtDir)
flightNumber = 1
(trials, times, sample_rate, channels) = FlightData.get_trials(flightNumber, 150, 150)

number_of_measurements = trials.shape[0]
number_of_channels = trials.shape[1]
number_of_trials = trials.shape[2]

# Morlet wavelet frequencies
frequencies = np.linspace(2, 30, 40)

# Baseline window used during db normalization
baseline_window = [-500, -200]

# Define list of number of cycles. Less cycles - better precision in time, more cycles - better precision in frequency.
cycles = [4, 6, 8, 15]

# Time for wavelet, should contain odd number of samples
time = np.arange(-2, 2, 1 / sample_rate)
if time[-1] != 2:
    time = np.append(time, 2)

# Calculate half of wavelet, will be used to trim result of convolution
half_wave = int((len(time) - 1) / 2)

# Calculate length of convolution
convolution_length = len(time) + (number_of_measurements * number_of_trials) - 1

for channel_index in range(0, len(channels)):
    channel_name = channels[channel_index]
    print(f"Performing Morlet Wavelet Transform for channel {channel_name}")

    # Prepare coefficients array that will contain result of convolution of channels with Morlet wavelet
    coefficients = np.zeros((len(cycles), len(frequencies), number_of_measurements))

    # Calculate indices for baseline window
    baseline_indices = np.zeros(2)
    baseline_indices[0] = find_nearest_indice(times, baseline_window[0])
    baseline_indices[1] = find_nearest_indice(times, baseline_window[1])

    # Calculate FFT for given channel
    dataX = np.fft.fft(np.reshape(trials[:, channel_index, :].transpose(), (-1)), convolution_length)

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
            convolvedSignal = np.fft.ifft(morlet_FFT * dataX)

            # Convolution results in output being longer that the original signal (output = N + M - 1)
            # Trim to the size of signal
            convolvedSignal = convolvedSignal[(half_wave):(-half_wave)]

            # Reshape 1D array to 2D in order to split trials
            convolvedSignal = np.reshape(convolvedSignal, (number_of_measurements, number_of_trials), order="F")

            # Calculate power
            coefficients[cycle_index, frequency_index, :] = np.mean(np.abs(convolvedSignal) ** 2, 1)

        # Perform db normalization
        baseline_mean = np.mean(coefficients[cycle_index, :, int(baseline_indices[0]):int(baseline_indices[1])], 1)
        coefficients[cycle_index, :, :] = 10 * np.log10(np.divide(coefficients[cycle_index, :, :], baseline_mean[:, np.newaxis]))

    # Plot results
    CustomFigure.get_custom_figure()
    gs = plt.GridSpec(len(cycles), 1)
    for cycle_index in range(0, len(cycles)):
        plt.subplot(2, 2, cycle_index + 1)
        plt.contourf(times, frequencies, np.squeeze(coefficients[cycle_index, :, :]), 40)
        plt.set_cmap('jet')
        plt.clim(-3, 3)
        plt.xlim([-400, 1000])
        plt.title(f"Liczba cykli falki: {cycles[cyclei]}")
        plt.xlabel('Czas [ms]')
        plt.ylabel('Częstotliwość [Hz]')
        plt.savefig(f"{wtDir}/WT{flightNumber}_{channel_name}.png", format='png', bbox_inches='tight')

