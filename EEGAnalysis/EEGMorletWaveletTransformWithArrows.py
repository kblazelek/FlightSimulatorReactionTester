import pywt
import numpy as np
import matplotlib.pyplot as plt
import scipy.fftpack
from pylab import *
from pandas import read_csv, DataFrame
from ArrowStateParser import *

# Read EEG data from CSV file
channelToRead = 1  # Which channel to read data from
data = read_csv('./Data/2018.17.8_14.31.16_EEG.csv', sep=";")
data = DataFrame(data)
arrowStates = data.values[:, 14]

arrowsShown = ArrowStateParser().get_times_when_arrow_appeared_on_screen(arrowStates)
y = data.values[:, channelToRead]  # 1st column contains time, channels start from 2nd column (index 1 in array)
N = y.size  # number of samples
fs = 128.0  # sampling rate
x = linspace(1, N, N) / fs  # time vector
T = 1 / fs

# Remove DC offset
y = y - np.mean(y)

# Prepare grid of subplots
fig = plt.figure()
gs = GridSpec(3, 1)
fig.subplots_adjust(hspace=0.8, wspace=0.8)

# Plot signal
fig.add_subplot(gs[0, 0])
plt.title(f'Signal from channel {channelToRead} without DC offset')
plt.xlabel('Time [s]')
plt.ylabel(r'Amplitude [$\mu V$]')
plt.plot(x, y)

# Calculate and plot Morlet wavelet transform
scales = np.arange(2, 130)
w = pywt.ContinuousWavelet('morl')
coef, freqs = pywt.cwt(y, scales, wavelet='morl', sampling_period=T)
fig.add_subplot(gs[1:, 0])
plt.title('Morlet wavelet transform of signal')
plt.xlabel('Time [s]')
plt.ylabel('Frequency [Hz]')
plt.set_cmap('jet')
plt.yticks(np.arange(np.floor(np.min(freqs)), np.ceil(np.max(freqs)), 2))
plt.contourf(x, freqs, coef.real, 40)
plt.colorbar()
arrowsShown = np.max(freqs) ** arrowsShown
plt.plot(x, arrowsShown, label='Times when arrow appeared on the screen')
plt.legend()
plt.show()
