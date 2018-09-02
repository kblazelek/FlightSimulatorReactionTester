import numpy as np
from xml.dom import minidom
# <?xml version="1.0"?>
# <FutureEventSetResult xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
#   <FutureEventResult>
#     <ReactionTimeMilliseconds>342.1515</ReactionTimeMilliseconds>
#     <FutureEvent>
#       <Arrow>Left</Arrow>
#       <Delay>60000</Delay>
#     </FutureEvent>
#   </FutureEventResult>
#   <FutureEventResult>
#     <ReactionTimeMilliseconds>293.54150000000004</ReactionTimeMilliseconds>
#     <FutureEvent>
#       <Arrow>Left</Arrow>
#       <Delay>60000</Delay>
#     </FutureEvent>
#   </FutureEventResult>
# </FutureEventSetResult>

def parse(file_path):
    """
    Parses xml containing information about reaction time to future events.
    :param file_path: Path to xml with future events
    :return: tuple of reaction times, arrows and delays
    """
    reactionTimesXML = minidom.parse(file_path)
    futureEventResults = reactionTimesXML.getElementsByTagName('FutureEventResult')
    reactionTimes = np.zeros(len(futureEventResults))
    delays = np.zeros(len(futureEventResults))
    arrows = []
    for i in range(0, len(futureEventResults)):
        futureEventResult = futureEventResults[i]
        reactionTimeMilliseconds = float(futureEventResult.getElementsByTagName("ReactionTimeMilliseconds")[0].firstChild.data)
        arrow = futureEventResult.getElementsByTagName("FutureEvent")[0].getElementsByTagName("Arrow")[0].firstChild.data
        delay = futureEventResult.getElementsByTagName("FutureEvent")[0].getElementsByTagName("Delay")[0].firstChild.data
        reactionTimes[i] = reactionTimeMilliseconds
        arrows.append(arrow)
        delays[i] = delay
    arrows = np.asarray(arrows)
    return reactionTimes, arrows, delays
