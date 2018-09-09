import numpy as np
import matplotlib.pyplot as plt
import FlightData
import CustomFigure
import os

# This script generates butterfly plots showing average data for all channels on single figure for selected arrows
bppaDir = './Images/BPPA'
if not os.path.exists(bppaDir):
    os.makedirs(bppaDir)
flightNumber = 1
removeArtifacts = True
arrows = ['Left', 'Right']

CustomFigure.get_custom_figure()
for arrow_index in range(0, len(arrows)):
    arrow_name = arrows[arrow_index]
    (trials, times, sample_rate, channels) = FlightData.get_trials_for_arrow(flightNumber, arrow_name, 150, 150)

    # Butterfly plot
    for channel_index in range(0, len(channels)):
        channel_name = channels[channel_index]
        channel_color = CustomFigure.get_color_for_channel(channel_name)
        plt.plot(times, np.mean(trials[:, channel_index, :], 1), label=channels[channel_index], color=channel_color)
        plt.xlabel('Czas [ms]')
        plt.ylabel(r'Amplituda [${\mu V}$]')
        plt.legend()
    if removeArtifacts:
        fileName = f"{bppaDir}/BPPA{flightNumber}_NoArtifacts_{arrow_name}.png"
    else:
        fileName = f"{bppaDir}/BPPA{flightNumber}_WithArtifacts_{arrow_name}.png"
    plt.savefig(fileName, format='png', bbox_inches='tight')
    plt.clf()
