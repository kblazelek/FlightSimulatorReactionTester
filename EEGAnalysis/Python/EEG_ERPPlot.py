import numpy as np
import matplotlib.pyplot as plt
import FlightData
import CustomFigure
import os

# This script creates ERP plots for all channels
erpDir = './Images/ERP'
if not os.path.exists(erpDir):
    os.makedirs(erpDir)
flightNumber = 3
removeArtifacts = True
(trials, times, sample_rate, channels) = FlightData.get_trials(flightNumber, 150, 150, removeArtifacts)

# ERP plot
for channelIndex in range(0, len(channels)):
    CustomFigure.get_custom_figure()
    plt.plot(times, trials[:, channelIndex, :], color='k')
    plt.plot(times, np.mean(trials[:, channelIndex, :], 1), linewidth=2, color='r')
    plt.xlabel('Czas [ms]')
    plt.ylabel(r'Amplituda [${\mu V}$]')
    if removeArtifacts:
        filename = f"{erpDir}/ERP{flightNumber}_{channels[channelIndex]}_NoArtifacts.png"
    else:
        filename = f"{erpDir}/ERP{flightNumber}_{channels[channelIndex]}_WithArtifacts.png"
    plt.savefig(filename, format='png', bbox_inches='tight')
#plt.show()
