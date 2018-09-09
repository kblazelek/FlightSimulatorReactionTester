import numpy as np
import matplotlib.pyplot as plt
import os

# This script demonstrates effect of cycles on gaussian and Morlet wavelet functions
def find_nearest_indice(array, value):
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return idx

def get_custom_figure():
    fig = plt.figure()
    fig.subplots_adjust(hspace=0.99, wspace=0.2, left=0.07, right=0.99, top=0.95, bottom=0.1)
    fig.set_size_inches(19.20, 10.80)
    fig.set_dpi(100)
    plt.rcParams.update({'font.size': 22})
    return fig


wtCyclesDir = './Images/WT_Cycles'
if not os.path.exists(wtCyclesDir):
    os.makedirs(wtCyclesDir)
# Define list of number of cycles. Less cycles - better precision in time, more cycles - better precision in frequency.
cycles = [4, 6, 8, 15]
centerFrequency = 6.5  # Center frequency of wavelet
sample_rate = 128.0

# Time for wavelet, should contain odd number of samples
time = np.arange(-2, 2, 1 / sample_rate)
if time[-1] != 2:
    time = np.append(time, 2)

# Create new figure
get_custom_figure()

for i in range(0, len(cycles)):
    plt.subplot(len(cycles), 1, i + 1)
    # Calculate standard deviation for gaussian function
    s = cycles[i] / (2 * np.pi * centerFrequency)

    # Create gaussian function
    gaussian = np.exp((-time ** 2) / (2 * s ** 2))

    # Plot gaussian function
    plt.plot(time, gaussian)
    plt.title(f"Krzywa Gaussa z {cycles[i]} cyklami")

# Name axis
plt.xlabel('Czas [s]')
plt.ylabel('Amplituda')

# Save figure to file
plt.savefig(f"{wtCyclesDir}/WTC_Gaussian.png", format='png', bbox_inches='tight')

# Create new figure
get_custom_figure()
for i in range(0, len(cycles)):
    plt.subplot(len(cycles), 1, i + 1)

    # Calculate standard deviation for gaussian function
    s = cycles[i] / (2 * np.pi * centerFrequency)

    # Create gaussian function
    gaussian = np.exp((-time ** 2) / (2 * s ** 2))

    # Create sinus function
    sinus = np.exp(1j * 2 * np.pi * centerFrequency * time)

    # Create complex Morlet wavelet by multiplying sinus and gaussian point by point
    morlet = sinus * gaussian

    # Take FFT of morlet wavelet and pad zeroes
    morlet_FFT = np.fft.fft(morlet)

    # Normalize FFT so that the biggest value is 1
    morlet_FFT = morlet_FFT / max(morlet_FFT)

    # Prepare frequencies for plot up to half of sample rate (Nyquist frequency)
    frequencies = np.linspace(0, sample_rate / 2, np.floor(len(time) / 2) + 1)

    # Plot power spectrum
    plt.plot(frequencies, np.abs(morlet_FFT[0: len(frequencies)]))
    plt.title(f"Widmo mocy falki Morleta z {cycles[i]} cyklami")
    plt.xlim((0, 20))
plt.xlabel('Częstotliwość [Hz]')
plt.ylabel('Gęstość mocy')
# Save figure to file
plt.savefig(f"{wtCyclesDir}/WTC_PS.png", format='png', bbox_inches='tight')
plt.show()
