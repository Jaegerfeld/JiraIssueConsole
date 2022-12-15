library(ggplot2)
library(ggrepel)
library(ggthemes)
library(plotly)
library(EnvStats)
library(ggstatsplot)
library(ggtext)

#Boxplot CycleTime Features 
maxdays <- summaryCycle[6]
standarddeviation <- sd(calcDT$CycleDays)
mad <- mad(calcDT$CycleDays)
spanwidth <- (summaryCycle[6]-summaryCycle[1])
title =  paste("CycleTime", "Min:", summaryCycle[1],
               "| Q1: ",  round(as.numeric(summaryCycle[2]), digits = 2),
               "| Mean: ", round(as.numeric(summaryCycle[4]), digits = 2),
               " | Median: ", round(as.numeric(summaryCycle[3]), digits = 2),
               " | Q3: ", round(as.numeric(summaryCycle[5]), digits = 2),
               " | Max: ", round(as.numeric(summaryCycle[6]), digits = 2),
			   "| #Items: ", nrow(calcDT),
			   "StdDev: ", round(as.numeric(standarddeviation), digits = 2),
               "MAD: ", round(as.numeric(mad), digits = 2))

boxplotCycledays <- ggplot(calcDT, aes(CycleDays)) + 
    geom_boxplot( varwidth = TRUE, fill = "orange", outlier.colour = "red", outlier.shape = 1, outlier.size =  2) +
    ggtitle(title)

dates <- as.Date(scatterTable$`Closed Date`, "%Y-%m-%d %H:%M:%S", tz = "Europe/London")

scatterpercentiles <- quantile(scatterTable$CycleDays, probs = c(.50, .85, .95))


scatterPlot <- ggplot(scatterTable, aes(x = dates, y = CycleDays, color = CycleDays)) +
    geom_point(shape = 1) +
    geom_hline(yintercept=(scatterpercentiles[1]), linetype="dashed", color = "red") +
    geom_hline(yintercept=(scatterpercentiles[2]), linetype="dashed", color = "cyan") +
    geom_hline(yintercept=(scatterpercentiles[3]), linetype="dashed", color = "green") +
    scale_color_gradientn(colors = c("#00FF00", "#0000FF", "#FF0000")) +
    geom_smooth(method = loess, alpha = 0.1, color = "blue") +
    geom_text_repel(aes(label = Key), size = 1) + 
    ggtitle(paste("Range: ", round(as.numeric(spanwidth), digits = 2) , 
                  "Stddev: ", round(as.numeric(standarddeviation), digits = 2), 
                  "MAD: ", round(as.numeric(mad), digits = 2)))

dateslead <- as.Date(calcDT$`Closed Date`, "%Y-%m-%d %H:%M", tz = "Europe/London")

scatterLeadPlot <- ggplot(calcDT, aes(x = dateslead, y = lead, color = lead)) +
    geom_point(shape = 1) +
    scale_color_gradientn(colors = c("#00FF00", "#0000FF", "#FF0000")) +
    geom_smooth(method = loess, alpha = 0.1, color = "blue") +
    geom_text_repel(aes(label = Key), size = 1)


plot(boxplotCycledays)
plot(scatterPlot)


