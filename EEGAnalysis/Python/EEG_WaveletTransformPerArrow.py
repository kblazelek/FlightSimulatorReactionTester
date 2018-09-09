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


wtpaDir = './Images/WTPA'
if not os.path.exists(wtpaDir):
    os.makedirs(wtpaDir)

arrows = ['Left', 'Right']
flightNumbers = [1, 2, 3]

# Morlet wavelet frequencies
frequencies = np.linspace(2, 30, 40)

# Define list of number of cycles. Less cycles - better precision in time, more cycles - better precision in frequency.
cycles = [6]

for flightNumber in flightNumbers:
    for arrow_index in range(0, len(arrows)):
        arrow_name = arrows[arrow_index]
        (trials, times, sample_rate, channels) = FlightData.get_trials_for_arrow(flightNumber, arrow_name, 150, 150)
        coefficients = MorletWavelet.transform(trials, times, sample_rate, channels, cycles, frequencies)

        # Plot results
        for channel_index in range(0, coefficients.shape[0]):
            channel_name = channels[channel_index]
            CustomFigure.get_custom_figure2x2(channel_index)
            plt.subplot(1, 2, arrow_index+1)
            plt.contourf(times, frequencies, np.squeeze(coefficients[channel_index, 0, :, :]), 40)
            plt.set_cmap('jet')
            plt.clim(-3, 3)
            plt.xlim([-400, 1000])
            plt.title(CustomFigure.get_polish_translation_for_arrow(arrow_name))
            plt.xlabel('Czas [ms]')
            plt.ylabel('Częstotliwość [Hz]')
            plt.colorbar()

    for channel_index in range(0, coefficients.shape[0]):
        channel_name = channels[channel_index]
        CustomFigure.get_custom_figure(channel_index)
        print(f"Saving result of Morlet Wavelet Transform on channel {channel_name}")
        plt.savefig(f"{wtpaDir}/WTPA{flightNumber}_{channel_name}.png", format='png', bbox_inches='tight')
        # Clear figure after drawing wavelet transform results for all arrows in current flight
        plt.clf()
