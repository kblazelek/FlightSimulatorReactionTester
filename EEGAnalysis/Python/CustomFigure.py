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

arrow_color_dictionary = {
    "Left": 'red',
    "Up": 'green',
    "Right": 'blue',
    "Down": 'cyan'
}

arrow_en_to_pl_dictionary = {
    "Left": 'Strzałka w lewo',
    "Up": 'Strzałka w górę',
    "Right": 'Strzałka w prawo',
    "Down": 'Strzałka w dół'
}


def get_custom_figure(id=None):
    if type(id) == int:
        fig = plt.figure(id)
    else:
        fig = plt.figure()
    fig.subplots_adjust(hspace=0.3, wspace=0.2, left=0.07, right=0.99, top=0.95, bottom=0.1)
    fig.set_size_inches(19.20, 10.80)
    fig.set_dpi(100)
    plt.rcParams.update({'font.size': 22})
    return fig


def get_custom_figure2x2(id=None):
    if type(id) == int:
        fig = plt.figure(id)
    else:
        fig = plt.figure()
    fig.subplots_adjust(hspace=0.44, wspace=0.2, left=0.07, right=0.99, top=0.95, bottom=0.1)
    fig.set_size_inches(19.20, 10.80)
    fig.set_dpi(100)
    plt.rcParams.update({'font.size': 22})
    return fig


def get_color_for_channel(channel_name):
    return channel_color_dictionary[channel_name]


def get_color_for_arrow(arrow_name):
    return arrow_color_dictionary[arrow_name]


def get_polish_translation_for_arrow(arrow_name):
    return arrow_en_to_pl_dictionary[arrow_name]
