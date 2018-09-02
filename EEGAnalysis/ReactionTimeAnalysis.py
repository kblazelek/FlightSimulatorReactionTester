import numpy as np
import matplotlib.pyplot as plt
import FlightData
import CustomFigure
import os
from pandas import DataFrame


def get_custom_figure():
    fig = plt.figure()
    fig.subplots_adjust(hspace=0.3, wspace=0.2, left=0.07, right=0.69, top=0.87, bottom=0.1)
    fig.set_size_inches(19.20, 10.80)
    fig.set_dpi(100)
    plt.rcParams.update({'font.size': 22})
    return fig


rtDir = './Images/RT'
if not os.path.exists(rtDir):
    os.makedirs(rtDir)
flightNumber = 3

removeArtifacts = False
reactionTimes, arrows, delays = FlightData.get_future_event_set_result(flightNumber)
time = np.zeros(len(reactionTimes))
time[0] = delays[0] / 1000
for i in range(0, len(reactionTimes)):
    time[i] = time[i - 1] + (delays[i] / 1000)

# Plot reaction times
get_custom_figure()
plt.scatter(time, reactionTimes)
mean_reaction_time = np.mean(reactionTimes)
min_reaction_time = np.min(reactionTimes)
max_reaction_time = np.max(reactionTimes)
median_reaction_time = np.median(reactionTimes)
print(f"Mean reaction time for all arrows: {mean_reaction_time}")
print(f"Min reaction time for all arrows: {min_reaction_time}")
print(f"Max reaction time for all arrows: {max_reaction_time}")
print(f"Median reaction time for all arrows: {median_reaction_time}")
plt.plot([0, np.max(time)], [mean_reaction_time, mean_reaction_time], 'g--', lw=1,
         label="Średni czas reakcji")
plt.plot([0, np.max(time)], [min_reaction_time, min_reaction_time], 'b--', lw=1,
         label="Minimalny czas reakcji")
plt.plot([0, np.max(time)], [max_reaction_time, max_reaction_time], 'r--', lw=1,
         label="Maksymalny czas reakcji")
plt.legend(loc='upper right', bbox_to_anchor=(1.5, 1.1), ncol=1, prop={'size': 18})
plt.xlabel('Czas [s]')
plt.ylabel(r'Czas reakcji [ms]')
plt.savefig(f"{rtDir}/RT{flightNumber}.png", format='png', bbox_inches='tight')

# Group by arrows (should get 4 groups - Left, Right, Up, Down)
arrows_data_frame = DataFrame(arrows)
grouped = arrows_data_frame.groupby(0)

# Create new figure
# CustomFigure.get_custom_figure()
get_custom_figure()

# Print reaction time for each arrow group in different color
for arrow_name, group in grouped:
    # Get reaction times for current arrow group
    reaction_times_for_arrow = reactionTimes[group.index]

    # Get time for current arrow group
    time_for_arrow = time[group.index]

    # Get translated arrow name to polish
    arrow_name_polish = CustomFigure.get_polish_translation_for_arrow(arrow_name)

    # Print arrow group
    arrow_color = CustomFigure.get_color_for_arrow(arrow_name)
    plt.scatter(time_for_arrow, reaction_times_for_arrow, c=arrow_color, label=arrow_name_polish)

    # Print min, max, mean reaction time for current arrow group
    mean_reaction_time = np.mean(reaction_times_for_arrow)
    print(f"Mean reaction time for {arrow_name} arrow: {mean_reaction_time}")
    plt.plot([0, np.max(time)], [mean_reaction_time, mean_reaction_time], '--', color=arrow_color, lw=1,
             label=f"{arrow_name_polish} - średni czas reakcji")
plt.xlabel('Czas [s]')
plt.ylabel(r'Czas reakcji [ms]')
plt.legend(loc='upper right', bbox_to_anchor=(1.5, 1.1), ncol=1, prop={'size': 18})
plt.savefig(f"{rtDir}/RT{flightNumber}_Grouped.png", format='png', bbox_inches='tight')
#plt.show()
