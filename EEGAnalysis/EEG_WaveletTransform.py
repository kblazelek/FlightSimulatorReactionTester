import numpy as np
import matplotlib.pyplot as plt
import FlightData
import os
import CustomFigure
import MorletWavelet

def find_nearest_indice(array, value):
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return idx


wtDir = './Images/WT'
if not os.path.exists(wtDir):
    os.makedirs(wtDir)
flightNumber = 1
(trials, times, sample_rate, channels) = FlightData.get_trials(flightNumber, 150, 150)

# Morlet wavelet frequencies
frequencies = np.linspace(2, 30, 40)

# Define list of number of cycles. Less cycles - better precision in time, more cycles - better precision in frequency.
cycles = [4, 6, 8, 15]

coefficients = MorletWavelet.transform(trials, times, sample_rate, channels, cycles, frequencies)
# Plot results
CustomFigure.get_custom_figure()
gs = plt.GridSpec(len(cycles), 1)
for channel_index in range(0, coefficients.shape[0]):
    channel_name = channels[channel_index]
    print(f"Saving result of Morlet Wavelet Transform on channel {channel_name}")
    for cycle_index in range(0, len(cycles)):
        plt.subplot(2, 2, cycle_index + 1)
        plt.contourf(times, frequencies, np.squeeze(coefficients[channel_index, cycle_index, :, :]), 40)
        plt.set_cmap('jet')
        plt.clim(-3, 3)
        plt.xlim([-400, 1000])
        plt.title(f"Liczba cykli falki: {cycles[cycle_index]}")
        plt.xlabel('Czas [ms]')
        plt.ylabel('Częstotliwość [Hz]')
        plt.savefig(f"{wtDir}/WT{flightNumber}_{channel_name}.png", format='png', bbox_inches='tight')
    plt.clf()

