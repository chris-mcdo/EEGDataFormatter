function [channels] = resample_file(fName)
%resample_file Converts file into uniformly sampled EEG data
%   Channels 1-16
load(fName, 'amplitudes', 'times')

t0 = times(1);
t = (times - t0)./1000;
r = 128;

n = size(amplitudes, 1);

[y1, ~] = resample(amplitudes(1,:), t, r);
yOut = zeros([n length(y1)]);
for i = 1:n
    [yOut(i,:),] = resample(amplitudes(i,:), t, r);
end

channels = yOut;

end

