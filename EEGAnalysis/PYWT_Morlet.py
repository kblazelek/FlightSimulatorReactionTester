import pywt
import numpy as np
import matplotlib.pyplot as plt


def find_nearest_index(array, value):
    n = [abs(i - value) for i in array]
    idx = n.index(min(n))
    return idx


def print_bands(time, f):
    f_bands = np.array([1, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40])
    for band in f_bands:
        id = find_nearest_index(f, band)
        closest = f[id]
        p1 = [min(time), max(time)]
        p2 = [id, id]
        plt.plot(p1, p2, label=f"{closest} Hz")
    plt.legend()


N = 600
T = 1.0 / 128.0
t = np.linspace(0.0, N * T, N)
sig = np.cos(2 * np.pi * 7 * t) + np.real(np.exp(-7 * (t - 0.4) ** 2) * np.exp(1j * 2 * np.pi * 2 * (t - 0.4)))
sig = (sig - min(sig)) / (max(sig) - min(sig))
plt.figure()
plt.plot(t, sig)
plt.figure()
widths = np.arange(2, 32)
cwtmatr, freqs = pywt.cwt(sig, widths, 'morl')
plt.imshow(cwtmatr, extent=[0, N * T, widths.min(), widths.max()], cmap='PRGn', aspect='auto',
           vmax=abs(cwtmatr).max(), vmin=-abs(cwtmatr).max())
frequencies = pywt.scale2frequency('morl', widths) / T
for i in range(0, widths.size - 1):
    print(f"{widths[i]}\t{frequencies[i]}")
print_bands(t, frequencies)
plt.show()
