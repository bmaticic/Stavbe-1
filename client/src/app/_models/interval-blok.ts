
export interface MojElektroRead15MinDto {
  intervalBlocks: IntervalBlok[]
  messageCreated: string
  usagePoint: string
}

export interface IntervalBlok {
  intervalReadings: IntervalReading[]
  readingType: string
}

export interface IntervalReading {
  readingQualities: any[]
  timestamp: string
  value: string
}

