import numpy as np
import matplotlib.pyplot as plt
import FlightData
import CustomFigure
import os

erppaDir = './Images/ERPPA'
if not os.path.exists(erppaDir):
    os.makedirs(erppaDir)
flightNumber = 1
removeArtifacts = True
arrows = ['Left', 'Right']
for arrow_index in range(0, len(arrows)):
    arrow_name = arrows[arrow_index]
    (trials, times, sample_rate, channels) = FlightData.get_trials_for_arrow(flightNumber, arrow_name, 150, 150)

    # ERP plot
    for channelIndex in range(0, len(channels)):
        CustomFigure.get_custom_figure2x2(channelIndex)
        #plt.plot(times, trials[:, channelIndex, :], color='k')
        plt.plot(times, np.mean(trials[:, channelIndex, :], 1), linewidth=2, color=CustomFigure.get_color_for_arrow(arrow_name), label=f"Lot {flightNumber}")
        plt.xlabel('Czas [ms]')
        plt.ylabel(r'Amplituda [${\mu V}$]')
for channelIndex in range(0, len(channels)):
    CustomFigure.get_custom_figure2x2(channelIndex)
    if removeArtifacts:
        filename = f"{erppaDir}/ERPPA{flightNumber}_{channels[channelIndex]}_NoArtifacts.png"
    else:
        filename = f"{erppaDir}/ERPPA{flightNumber}_{channels[channelIndex]}_WithArtifacts.png"
    plt.savefig(filename, format='png', bbox_inches='tight')
    plt.close()
#plt.show()
