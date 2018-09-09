function [output] = GetArrowData(flight_number)
% This function returns arrows (Left, Right, Up, Down) that were presented
% in each trial. Each flight used the same set of arrows.
if(flight_number == 1)
    tree = xml2struct('.\..\..\..\Data\Flight1_ReactionTimes.xml');
elseif(flight_number == 2)
    tree = xml2struct('.\..\..\..\Data\Flight2_ReactionTimes.xml');
elseif(flight_number == 3)
    tree = xml2struct('.\..\..\..\Data\Flight3_ReactionTimes.xml');
end

futureEventResults = tree.FutureEventSetResult.FutureEventResult;
arrows = cell(1, length(futureEventResults));
for i=1:length(futureEventResults)
    futureEventResult = futureEventResults(i);
    arrow = futureEventResult{1}.FutureEvent.Arrow;
    arrows{i} = arrow.Text;
end
output = arrows;
end