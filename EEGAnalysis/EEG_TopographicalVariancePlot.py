import numpy as np
import matplotlib.pyplot as plt
import FlightData
import CustomFigure
import os


tvDir = './Images/TV'
if not os.path.exists(tvDir):
    os.makedirs(tvDir)
flightNumber = 1
removeArtifacts = True
(trials, times, sample_rate, channels) = FlightData.get_trials(flightNumber, 150, 150, removeArtifacts)

# Topographical variance plot
CustomFigure.get_custom_figure()
plt.plot(times, np.var(np.mean(trials, 2), 1))
plt.xlabel('Czas [ms]')
plt.ylabel(r'Wariancja amplitudy [${\mu V}$]')
if removeArtifacts:
    filename = f"{tvDir}/TV{flightNumber}_NoArtifacts.png"
else:
    filename = f"{tvDir}/TV{flightNumber}_WithArtifacts.png"
plt.savefig(filename, format='png', bbox_inches='tight')
plt.show()
