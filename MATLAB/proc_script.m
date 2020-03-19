expNo = 8;

rootdir = 'C:\Users\cmcdo\Documents\MATLAB\BA project\Data\';

inputdir = [rootdir int2str(expNo) '_unproc\'];

mkdir(rootdir, [int2str(expNo) '_resampled\'])
outputdir = [rootdir int2str(expNo) '_resampled\'];

pathList = dir(fullfile(inputdir,'*.mat'));
for k = 1:numel(pathList)
    fName = pathList(k).name;
    fullPath = fullfile(inputdir,pathList(k).name);
    channels = resample_file(fullPath);
    save([outputdir fName], 'channels')
end