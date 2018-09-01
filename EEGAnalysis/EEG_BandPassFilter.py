from scipy.signal import *
from pandas import read_csv, DataFrame
from pylab import *


def butter_bandpass(lowcut, highcut, fs, order=5):
    nyq = 0.5 * fs
    low = lowcut / nyq
    high = highcut / nyq
    b, a = butter(order, [low, high], btype='band')
    return b, a


def butter_bandpass_filter(data, lowcut, highcut, fs, order=5):
    b, a = butter_bandpass(lowcut, highcut, fs, order=order)
    y = lfilter(b, a, data)
    return y


def fir_filter(data, lowcut, highcut):
    noTaps = 466
    filt = firwin(noTaps, [lowcut, highcut], pass_zero=False, window=('kaiser', 5.6533), nyq=64)
    y = filtfilt(filt, 1, data)
    return y


if __name__ == "__main__":
    import numpy as np
    import matplotlib.pyplot as plt
    from scipy.signal import freqz

    fs = 128.0  # sample rate
    data = read_csv('Data/2018.08.20_13.59.46_EEG.csv', sep=";")
    data = DataFrame(data)
    # Get all rows of 2nd column
    x = data.values[:, 2]
    N = x.size  # number of samples
    t = linspace(1, N, N) / fs  # time vector

    # Filter a noisy signal.
    f_bands = np.array([[1, 4], [4, 8], [8, 12], [12, 16], [16, 20], [20, 24], [24, 28], [32, 36], [36, 40], [8, 30]])
    no_bands = f_bands.shape[0]
    plt.figure()
    plt.clf()
    plt.plot(t, x, label='EEG signal')

    for fid in range(0, no_bands):
        lowcut = f_bands[fid, 0]
        highcut = f_bands[fid, 1]
        print(f"Filtering signal band [{lowcut}, {highcut}]")
        # y = butter_bandpass_filter(x, lowcut, highcut, fs, order=6)
        y = fir_filter(x, lowcut, highcut)
        plt.plot(t, y, label=f"Band [{lowcut}, {highcut}]")
        plt.xlabel('time (seconds)')
        plt.grid(True)
        plt.axis('tight')
        plt.legend(loc='upper left')
    plt.show()
