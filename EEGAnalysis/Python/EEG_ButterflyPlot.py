import numpy as np
import matplotlib.pyplot as plt
import FlightData
import CustomFigure
import os

# This script generates butterfly plots showing average data for all channels on single figure
bpDir = './Images/BP'
if not os.path.exists(bpDir):
    os.makedirs(bpDir)
flightNumber = 3
removeArtifacts = False
(trials, times, sample_rate, channels) = FlightData.get_trials(flightNumber, 150, 150, removeArtifacts)

# Butterfly plot
CustomFigure.get_custom_figure()
for channel_index in range(0, len(channels)):
    channel_name = channels[channel_index]
    channel_color = CustomFigure.get_color_for_channel(channel_name)
    plt.plot(times, np.mean(trials[:, channel_index, :], 1), label=channels[channel_index], color=channel_color)
    plt.xlabel('Czas [ms]')
    plt.ylabel(r'Amplituda [${\mu V}$]')
    plt.legend()
if removeArtifacts:
    fileName = f"{bpDir}/BP{flightNumber}_NoArtifacts.png"
else:
    fileName = f"{bpDir}/BP{flightNumber}_WithArtifacts.png"
plt.savefig(fileName, format='png', bbox_inches='tight')
plt.show()
