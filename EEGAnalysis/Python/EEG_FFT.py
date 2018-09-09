import numpy as np
import matplotlib.pyplot as plt
import CustomFigure
import os
from pandas import read_csv

fftDir = './Images/FFT'
if not os.path.exists(fftDir):
    os.makedirs(fftDir)
sample_rate = 128.0
data = read_csv('./Data/2018.08.20_13.59.46_EEG.csv', sep=";")

# Get EEG data for channel with index 0
channel_data = data.values[:, 0]
N = len(channel_data)  # Number of measurements

# Remove DC offset
channel_data = channel_data - np.mean(channel_data)

# Perform FFT
fft = np.fft.fft(channel_data)

# Prepare x axis (frequencies)
f = np.linspace(0, sample_rate, N)

# Fourier plot (power spectrum)
CustomFigure.get_custom_figure()
plt.plot(f[:N // 2], np.abs(fft)[:N // 2] * 1 / N)  # 1 / N is a normalization factor
plt.xlabel('Częstotliwość [Hz]')
plt.ylabel('Gęstość mocy')
plt.savefig(f"{fftDir}/PS.png", format='png', bbox_inches='tight')
# plt.show()
