


implmins <- round((as.numeric(calcDT$`In Progress`)
                  ), digits = 1)


analysismins <- round((as.numeric(calcDT$`In Specification`)), digits = 1);

backlogmins <- round((as.numeric(calcDT$Open)), digits = 1);

blockedmins <- round((as.numeric(calcDT$Pending)), digits = 1);


calcDT$FunnelDays <- as.numeric(calcDT$New) / 1440
calcDT$DevTime <- implmins / 1440
calcDT$BlockedTimeDays <- blockedmins / 1440
calcDT$AnalysisDays <- analysismins / 1440
calcDT$BacklogDays <- backlogmins / 1440

calcDT$Pending[calcDT$Pending == 0] <- NA
