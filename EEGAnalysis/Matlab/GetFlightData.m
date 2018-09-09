function [output] = GetFlightData(flight_number)
% This function loads EEG data in EEGLAB format for flight 1, 2 or 3
% Trials and channels that contained artifacts are removed
% EEG.data is of size channels x measurements x trials

% Remove bad trials
if(flight_number == 1)
    % Load data into EEG variable
    load .\..\..\..\Data\Flight1_EEGLAB.mat
    
    % Indexes of channels to keep
    channelIndexesToKeep = ones(size(EEG.chanlocs, 2), 1);
    channelIndexesToKeep(10) = 0;
    
    % Indexes of trials to be removed
    trialIndexesToRemove = [0 1 2 4 8 22 24 25 26 28 29 38 43 44 45 51] + 1;
elseif(flight_number == 2)
    % Load data into EEG variable
    load .\..\..\..\Data\Flight2_EEGLAB.mat
    
    % Indexes of channels to keep
    channelIndexesToKeep = ones(size(EEG.chanlocs, 2), 1);
    channelIndexesToKeep(10) = 0;
    
    % Indexes of trials to be removed
    trialIndexesToRemove = [13 14 25 36 37 38 51] + 1;
elseif(flight_number == 3)
    % Load data into EEG variable
    load .\..\..\..\Data\Flight3_EEGLAB.mat
    
    % Indexes of channels to keep
    channelIndexesToKeep = ones(size(EEG.chanlocs, 2), 1);
    channelIndexesToKeep(12) = 0;
    
    % Indexes of trials to be removed
    trialIndexesToRemove = [0 1 3 4 7 8 9 10 11 13 14 15 16 17 18 19 21 22 23 24 25 26 29 36 37 38 39 42 45 46 55 56] + 1;
else
    error("Wrong flight number");
end

% Remove channels with artifacts
EEG.chanlocs = EEG.chanlocs(1, logical(channelIndexesToKeep));
EEG.data = EEG.data(logical(channelIndexesToKeep),:,:);

% Remove trials with artifacts
trialIndexesToKeep = ones(size(EEG.data, 3), 1);
trialIndexesToKeep(trialIndexesToRemove) = 0;
EEG.data = EEG.data(:,:,logical(trialIndexesToKeep));

sizeOfEEG = size(EEG.data);
EEG.nbchan = sizeOfEEG(1);
EEG.trials = sizeOfEEG(3);
output = EEG;
end