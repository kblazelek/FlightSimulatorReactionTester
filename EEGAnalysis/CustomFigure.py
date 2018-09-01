import matplotlib.pyplot as plt

channel_color_dictionary = {
    "AF3": 'red',
    "F7": 'green',
    "F3": 'blue',
    "FC5": 'cyan',
    "T7": 'magenta',
    "P7": 'yellow',
    "O1": 'black',
    "O2": 'lime',
    "P8": 'brown',
    "T8": 'pink',
    "FC6": 'orange',
    "F4": 'navy',
    "F8": 'salmon',
    "AF4": 'gray',
}


def get_custom_figure():
    fig = plt.figure()
    fig.subplots_adjust(hspace=0.3, wspace=0.2, left=0.07, right=0.99, top=0.95, bottom=0.1)
    fig.set_size_inches(19.20, 10.80)
    fig.set_dpi(100)
    plt.rcParams.update({'font.size': 22})
    return fig


def get_color_for_channel(channel_name):
    return channel_color_dictionary[channel_name]

