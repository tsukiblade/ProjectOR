using CommandLine;

namespace Voxel;

public class Options
{
    [Option('p', "parallel", Required = false, HelpText = "Number of parallel tasks. Default is 0. -1 means all cores.")]
    public int Parallel { get; set; } = 0;
    
    [Option('f', "file", Required = true, HelpText = "Input file to be processed.")]
    public string FileName { get; set; } = default!;
    
    [Option('x', "x-dim", Required = true, HelpText = "X dimension of the voxel grid.")]
    public int XDim { get; set; } = default!;
    
    [Option('y', "y-dim", Required = true, HelpText = "Y dimension of the voxel grid.")]
    public int YDim { get; set; } = default!;
    
    [Option('z', "z-dim", Required = true, HelpText = "Z dimension of the voxel grid.")]
    public int ZDim { get; set; } = default!;
    
    [Option('o', "output", Required = false, HelpText = "Should save output file.")]
    public bool ShouldSaveOutput { get; set; } = false;
}