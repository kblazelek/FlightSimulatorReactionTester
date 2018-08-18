import pywt
import numpy as np
import matplotlib.pyplot as plt
import scipy.fftpack

N = 1280  # Number of samplepoints
f = 128.0  # sample frequency in Hz
T = 1.0 / f  # sample spacing

# Prepare signal
x = np.linspace(0.0, N * T, N)
y = np.zeros(x.size)
for i in range(0, x.size - 1):
    if x[i] < 3:
        y[i] = np.sin(4.0 * 2.0 * np.pi * x[i])  # 4 Hz sinusoid (0-3S)
    elif x[i] < 6:
        y[i] = np.sin(8.0 * 2.0 * np.pi * x[i])  # 8 Hz sinusoid (3-6S)
    else:
        y[i] = np.sin(12.0 * 2.0 * np.pi * x[i])  # 12 Hz sinusoid (6-10S)

# Plot original signal
fig = plt.figure()
fig.subplots_adjust(hspace=0.4, wspace=0.4)
plt.subplot(2, 1, 1)
plt.title('Original signal')
plt.xlabel('Time [s]')
plt.ylabel('Amplitude')
plt.plot(x, y)

# Calculate and show FFT of signal
yf = scipy.fftpack.fft(y)
xf = np.linspace(0.0, 1.0 / (2.0 * T), N / 2)
plt.subplot(2, 1, 2)
plt.title('FFT of signal')
plt.xlabel('Frequency [Hz]')
plt.ylabel('Amplitude')
plt.plot(xf, 2.0 / N * np.abs(yf[:N // 2]))

# Perform Morlet Wavelet Transform
# Scales stretch/shrink wavelets. Higher scales are better at detecting lower frequencies.
# Scales can be mapped to frequencies, i.e.
# frequencies = pywt.scale2frequency('morl', [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20]) / T
# Coefficients are a result of wavelet transform
# Frequencies are resulting from scales and are scaled to real values when passing sampling_period to pywt.cwt
scales = np.arange(1, 129)
coef, freqs = pywt.cwt(y, scales, 'morl', sampling_period=T)
plt.figure()
plt.title('Morlet wavelet transform of signal')
plt.xlabel('Time [s]')
plt.ylabel('Frequency [Hz]')
plt.set_cmap('jet')
plt.yticks(np.arange(np.floor(np.min(freqs)), np.ceil(np.max(freqs)), 2))
plt.contourf(x, freqs, coef, 40)
plt.colorbar()
plt.show()
