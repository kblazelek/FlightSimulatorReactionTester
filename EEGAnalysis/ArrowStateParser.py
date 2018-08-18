from pylab import *

# ArrowState could be:
# 0 - arrow is invisible
# 1 - arrow is visible
# 2 - user clicked the correct arrow


class ArrowStateParser:
    def get_times_when_arrow_appeared_on_screen(self, arrow_states):
        """
        Returns times when arrow appeared on the screen.
        I.e. for arrow states [0, 0, 1, 1, 1, 2, 0] returns [0, 0, 1, 0, 0, 0, 0]
        :param arrow_states: 1d array of arrow states of values 0, 1 or 2
        :return: 1d array with 1'ns at times when arrow appeared on the screen
        """
        arrows_shown = np.zeros(arrow_states.size)
        if arrow_states[0] == 1:
            arrows_shown[0] = 1
        for i in range(1, arrow_states.size - 1):
            if arrow_states[i] == 1 and arrow_states[i - 1] == 0:
                arrows_shown[i] = 1
        return arrows_shown
