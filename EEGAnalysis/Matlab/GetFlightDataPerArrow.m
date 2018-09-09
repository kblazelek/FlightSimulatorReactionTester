function [output] = GetFlightDataPerArrow(flight_number, arrow_name)
% This function loads EEG data in EEGLAB format for flight 1, 2 or 3 and
% returns only trials in which arrow_name was shown (Left, Right, Up or Down)
% Trials and channels that contained artifacts are removed

EEG = GetFlightData(flight_number);
arrows = GetArrowData(flight_number);

% Remove arrows that resulted from faulty trials
if(flight_number == 1)
    % Indexes of trials to be removed
    trialIndexesToRemove = [0 1 2 4 8 22 24 25 26 28 29 38 43 44 45 51] + 1;
elseif(flight_number == 2)
    % Indexes of trials to be removed
    trialIndexesToRemove = [13 14 25 36 37 38 51] + 1;
elseif(flight_number == 3)
    % Indexes of trials to be removed
    trialIndexesToRemove = [0 1 3 4 7 8 9 10 11 13 14 15 16 17 18 19 21 22 23 24 25 26 29 36 37 38 39 42 45 46 55 56] + 1;
else
    error("Wrong flight number");
end
indexes = ones(size(arrows, 2), 1);
indexes(trialIndexesToRemove) = 0;
arrows = arrows(1,logical(indexes));

% Leave only arrows with requested name, i.e. Left
indexes_to_keep = strcmp(arrows, arrow_name);
EEG.data = EEG.data(:,:,indexes_to_keep);
output = EEG;
end
