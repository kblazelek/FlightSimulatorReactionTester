EEG1 = GetFlightData(1);
EEG2 = GetFlightData(2);
EEG3 = GetFlightData(3);
channelCount = size(EEG3.data, 1);
outputDir = 'TopoPlots';
mkdir TopoPlots
timepoints = -1100:10:1100;
flightNumber = 1;
figure('rend','painters','pos',[10 10 900 600])
%figure('units','normalized','outerposition',[0 0 1 1])
for i = 1:length(timepoints)
    timepoint = timepoints(i);
    % convert time point from ms to index
    [~,timepointidx1] = min(abs(EEG1.times-timepoint));
    [~,timepointidx2] = min(abs(EEG2.times-timepoint));
    [~,timepointidx3] = min(abs(EEG3.times-timepoint));

    % data from this frequency from some other matrix
    dat1 = double( mean(EEG1.data(:,timepointidx1,:),3) );
    dat1 = dat1 - mean(dat1);
    
    dat2 = double( mean(EEG2.data(:,timepointidx2,:),3) );
    dat2 = dat2 - mean(dat2);
    
    dat3 = double( mean(EEG3.data(:,timepointidx3,:),3) );
    dat3 = dat3 - mean(dat3);

    % eeglab function
    subplot(131)
    topoplot(dat1,EEG1.chanlocs,'electrodes','ptslabels'); % eeglab's topoplot function
    title(strcat('Lot 1 (', num2str(timepoint), ' ms)'))
    colorbar
    subplot(132)
    topoplot(dat2,EEG2.chanlocs,'electrodes','ptslabels'); % eeglab's topoplot function
    title(strcat('Lot 2 (', num2str(timepoint), ' ms)'))
    colorbar
    subplot(133)
    topoplot(dat3,EEG3.chanlocs,'electrodes','ptslabels'); % eeglab's topoplot function
    title(strcat('Lot 3 (', num2str(timepoint), ' ms)'))
    colorbar
    fileName = outputDir + "/" + num2str(i,'%03.f') + '_TP' + '_' + timepoint + ".png";
    %print(gcf,fileName,'-dpng','-r0')
    export_fig(fileName,'-p0.01');
    %saveas(gcf, fileName)
    clf
end

%% useful bit of code to see channel locations

%figure
%topoplot([],EEG.chanlocs,'electrodes','ptslabels','hcolor','magenta');
% hint: click on an electrode to see its corresponding index (number)

%% end.
