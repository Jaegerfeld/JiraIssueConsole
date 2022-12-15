calcDTAging <- Features_ALL_DT


##cleanup 
# remove no Status Lines (== not an issue) , remove already closed Issues (== has a closed date, regarding to Workflow definition in issuecoll)

calcDTAging <- calcDTAging[!(is.na(calcDTAging$Status)),]
calcDTAging <- calcDTAging[(is.na(calcDTAging$`Closed Date`)),]

cycleAgingmins <- round((as.numeric(calcDTAging$`In Specification`)
                    + as.numeric(calcDTAging$Open)
                    + as.numeric(calcDTAging$`In Progress`)
                    + as.numeric(calcDTAging$`In Review`)
                    + as.numeric(calcDTAging$Pending)), digits = 1);

calcDTAging$CycleDays <- round(as.numeric(cycleAgingmins / 1440), digits = 1);

Funnel_Aging_DT <- subset(calcDTAging, calcDTAging$Status == "New")
NotReady_Aging_DT <- subset(calcDTAging, calcDTAging$Status != "New")

CountDoneIssues <- nrow(calcDT)
#"| Mean: ", round(as.numeric(summaryCycle[4]), digits = 2),
#" | Median: ", round(as.numeric(summaryCycle[3]), digits = 2),
captionTitle =  paste("CycleTime", 
               "| Mean: ", round(as.numeric(summaryCycle[4]), digits = 2),
               " | Median: ", round(as.numeric(summaryCycle[3]), digits = 2),
               "| # Done Items: ", nrow(calcDT))
summaryNotReady <- summary(NotReady_Aging_DT$CycleDays)

Reporttitle <- paste("Age & Current Status:  
                     Mean" , round(as.numeric(summaryNotReady[4]), digits = 2),
                     " | Median: ", round(as.numeric(summaryNotReady[3]), digits = 2),
                     "| # Not done Items: ", nrow(NotReady_Aging_DT))


AgingWIPPlot <- ggplot(NotReady_Aging_DT, aes(Status, CycleDays)) + 
  geom_boxplot() + 
  geom_dotplot(binaxis='y', 
               stackdir='center', 
               dotsize = .5,
               binwidth = 15,
               fill="red") +
  geom_hline(yintercept=(summaryCycle[3]), linetype="dashed", color = "red") +
  theme(legend.position = "none")  +
  scale_x_discrete(limits = c("Pending", "In Specification","Open","In Progress", "In Review")) +
  theme(axis.text.x = element_text(angle=65, vjust=0.6)) + 
   labs(title="Aging Work in Progress", 
        subtitle=Reporttitle,
        caption=captionTitle,
        x="Current Status",
        y="Total Age")


plot(AgingWIPPlot)

