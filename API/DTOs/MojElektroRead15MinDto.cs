using System;

namespace API.DTOs;

public class MojElektroRead15MinDto
{
    public List<IntervalBlock>? IntervalBlocks { get; set; }
    public string? MessageCreated { get; set; }
    public string? UsagePoint { get; set; }


}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

// IntervalBlock myDeserializedClass = JsonConvert.DeserializeObject<IntervalBlock>(myJsonResponse);
public class IntervalBlock
{
    public List<IntervalReading>? IntervalReadings { get; set; }
    public string? ReadingType { get; set; }
}

public class IntervalReading
{
    public List<object>? ReadingQualities { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Value { get; set; }
}


