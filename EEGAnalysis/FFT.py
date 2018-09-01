import numpy as np
from matplotlib import pyplot as plt
import os


def get_custom_figure():
    fig = plt.figure()
    fig.subplots_adjust(hspace=0.3, wspace=0.2, left=0.07, right=0.99, top=0.95, bottom=0.1)
    fig.set_size_inches(19.20, 10.80)
    fig.set_dpi(100)
    plt.rcParams.update({'font.size': 22})
    return fig


t = np.linspace(0, 1, 500)
s = np.sin(40 * 2 * np.pi * t) + 0.5 * np.sin(90 * 2 * np.pi * t)
fftDir = './Images/FFT'
if not os.path.exists(fftDir):
    os.makedirs(fftDir)
get_custom_figure()
plt.ylabel("Amplituda")
plt.xlabel("Czas [s]")
#plt.title('Analizowany sygnał')
plt.plot(t, s)
plt.savefig(f"{fftDir}/FFT1.png", format='png', bbox_inches='tight')
fft = np.fft.fft(s)
T = t[1] - t[0]  # sample rate
N = s.size
f = np.linspace(0, 1 / T, N)

get_custom_figure()
#plt.title('Część rzeczywista widma')
plt.ylabel("Amplituda")
plt.xlabel("Częstotliwość [Hz]")
plt.plot(f[:N // 2], fft.real[:N // 2] * 1 / N)  # 1 / N is a normalization factor
plt.savefig(f"{fftDir}/FFT2.png", format='png', bbox_inches='tight')

get_custom_figure()
#plt.title('Część urojona widma')
plt.ylabel("Amplituda")
plt.xlabel("Częstotliwość [Hz]")
plt.plot(f[:N // 2], fft.imag[:N // 2] * 1 / N)  # 1 / N is a normalization factor
plt.savefig(f"{fftDir}/FFT3.png", format='png', bbox_inches='tight')

get_custom_figure()
#plt.title('Widmo mocy sygnału')
plt.ylabel("Gęstość mocy")
plt.xlabel("Częstotliwość [Hz]")
plt.plot(f[:N // 2], np.abs(fft)[:N // 2] * 1 / N)  # 1 / N is a normalization factor
plt.savefig(f"{fftDir}/FFT4.png", format='png', bbox_inches='tight')
plt.show()
