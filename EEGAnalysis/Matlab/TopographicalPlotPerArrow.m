% This scripts requires EEGLAB toolset and export_fig

% Which arrow data to get from EEG (Left, Right, Up, Down)
arrow = 'Left';

% Timepoints at which topoplot should be generated (in ms)
timepoints = -400:50:1100; 

% Get EEG data for all 3 flights
EEG1 = GetFlightDataPerArrow(1, arrow);
EEG2 = GetFlightDataPerArrow(2, arrow);
EEG3 = GetFlightDataPerArrow(3, arrow);

% Create output directories
outputDir = 'TopoPlotsPerArrow';
mkdir TopoPlotsPerArrow
mkdir TopoPlotsPerArrow//Left
mkdir TopoPlotsPerArrow//Right
mkdir TopoPlotsPerArrow//Up
mkdir TopoPlotsPerArrow//Down

% Configure figure size so that output image is readable
figure('rend','painters','pos',[10 10 900 600])
for i = 1:length(timepoints)
    timepoint = timepoints(i);
    
    % convert time point from ms to index
    [~,timepointidx1] = min(abs(EEG1.times-timepoint));
    [~,timepointidx2] = min(abs(EEG2.times-timepoint));
    [~,timepointidx3] = min(abs(EEG3.times-timepoint));

    % Calculate mean of EEG for given timepoint (flight 1)
    dat1 = double( mean(EEG1.data(:,timepointidx1,:),3) );
    dat1 = dat1 - mean(dat1);
    
    % Calculate mean of EEG for given timepoint (flight 2)
    dat2 = double( mean(EEG2.data(:,timepointidx2,:),3) );
    dat2 = dat2 - mean(dat2);
    
    % Calculate mean of EEG for given timepoint (flight 3)
    dat3 = double( mean(EEG3.data(:,timepointidx3,:),3) );
    dat3 = dat3 - mean(dat3);
    
    % Plot topographical maps
    subplot(131)
    topoplot(dat1,EEG1.chanlocs,'electrodes','ptslabels');
    title(strcat('Lot 1 (', num2str(timepoint), ' ms)'))
    colorbar
    subplot(132)
    topoplot(dat2,EEG2.chanlocs,'electrodes','ptslabels');
    title(strcat('Lot 2 (', num2str(timepoint), ' ms)'))
    colorbar
    subplot(133)
    topoplot(dat3,EEG3.chanlocs,'electrodes','ptslabels');
    title(strcat('Lot 3 (', num2str(timepoint), ' ms)'))
    colorbar
    fileName = outputDir + "/" + arrow + "/" + num2str(i,'%03.f') + '_TPPA' + '_' + timepoint + '_' + arrow + ".png";
    export_fig(fileName,'-p0.01');
    clf
end
