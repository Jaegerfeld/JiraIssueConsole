
##import
library(tidyverse)


Enddate <- max(FeaturesCFD_DT$Day)
SD <- min(FeaturesCFD_DT$Day)
startDate <- Enddate - 365
lastDate <- Enddate


#laufende summe bilden

calcCFD_DT <- FeaturesCFD_DT

calcCFD_DT <- calcCFD_DT %>% filter( calcCFD_DT$Day >= startDate)

calcCFD_DT$X11 <- NULL


 calcCFD_DT$Funnel <- c(0, cumsum(calcCFD_DT$Funnel)[-nrow(calcCFD_DT)])
 calcCFD_DT$`In Analysis` <- c(0, cumsum(calcCFD_DT$`In Analysis`)[-nrow(calcCFD_DT)])
 calcCFD_DT$Backlog <- c(0, cumsum(calcCFD_DT$Backlog)[-nrow(calcCFD_DT)])
 calcCFD_DT$`In Implementation` <- c(0, cumsum(calcCFD_DT$`In Implementation`)[-nrow(calcCFD_DT)])
 calcCFD_DT$`E2E Test` <- c(0, cumsum(calcCFD_DT$`E2E Test`)[-nrow(calcCFD_DT)])
 calcCFD_DT$Completed <- c(0, cumsum(calcCFD_DT$Completed)[-nrow(calcCFD_DT)])
 calcCFD_DT$Monitoring <- c(0, cumsum(calcCFD_DT$Monitoring)[-nrow(calcCFD_DT)])
 calcCFD_DT$Done <- c(0, cumsum(calcCFD_DT$Done)[-nrow(calcCFD_DT)])
 calcCFD_DT$Blocker <- c(0, cumsum(calcCFD_DT$Blocker)[-nrow(calcCFD_DT)])


##### plotting

library(reshape2)
df <- calcCFD_DT
df <- melt(df, id.vars='Day')

dffirstStartAll = filter(df,(Day == startDate) 
                        & (df$variable == "In Analysis" | df$variable == "Backlog" 
                           | df$variable == "In Implementation" 
                            | df$variable == "Blocker" 
                              | df$variable == "E2E Test"
                               | df$variable == "Completed" 
                                | df$variable == "Monitoring" 
                                  | df$variable == "Done" 
                        )
                      )

dffirstStart = filter(df,(Day == startDate) & (df$variable == "In Analysis"))
dffirstStart$value <- sum(dffirstStartAll$value)

dflastStartAll = filter(df,(Day == lastDate) 
                        & (df$variable == "In Analysis" | df$variable == "Backlog" 
                           | df$variable == "In Implementation" 
                           | df$variable == "Blocker" 
                           | df$variable == "E2E Test"
                           | df$variable == "Completed" 
                           | df$variable == "Monitoring" 
                           | df$variable == "Done" 
                        )
)

dflastStart = filter(df,(Day == lastDate) & ( df$variable == "In Analysis"))
dflastStart$value <- sum(dflastStartAll$value)

dfStart <- rbind(dffirstStart,dflastStart)

dffirstEndAll = filter(df,(Day == startDate) & ( df$variable == "Done"
                                              | df$variable == "Monitoring"
                                              | df$variable == "Completed"
                                              )
                    )
dffirstEnd = filter(df,(Day == startDate) & (df$variable == "Completed"))
dffirstEnd$value <- sum(dffirstEndAll$value)



dflastEndAll = filter(df,(Day == lastDate) & ( df$variable == "Done"
                                             | df$variable == "Monitoring"
                                             | df$variable == "Completed"
)
)

dflastEnd = filter(df,(Day == lastDate) & (df$variable == "Completed"))
dflastEnd$value <- sum(dflastEndAll$value)

dfEnd <- rbind(dffirstEnd,dflastEnd)

ratio = dflastStart$value / dflastEnd$value

title = paste("Ratio In/out ", round(ratio,2) , " : 1")

cfdPlot <- ggplot(df, aes(x = Day, y = value)) +
  geom_area(aes(fill = variable)) +
  geom_line(data=dfStart, aes(x=Day, y=value, group=1)) +
  geom_line(data=dfEnd, aes(x=Day, y=value, group=1)) +
  labs(title = title,
       x = "Date",
       y = "Count")


plot(cfdPlot)
write.csv(calcCFD_DT, "Export/CFD-cumsum.csv", row.names = FALSE)
write.csv(df, "Export/CFD-cumsumDF.csv", row.names = FALSE)


filepathCFD <-  gsub(" ", "",paste("Export/CFD_", teamname))
pdf(file =  gsub(" ", "",paste(filepathCFD,".pdf")), width = 20,height = 8, onefile = TRUE)

print(cfdPlot)     #CFD Diagramm Plot

dev.off() 

