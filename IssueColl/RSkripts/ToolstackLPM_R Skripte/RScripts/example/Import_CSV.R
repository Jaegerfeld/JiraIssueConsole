library(readr)

filepath_Import <- gsub(" ", "", paste("Export/CSV/",teamname,"_IssueTimes.csv"))

Features_DT <- read_csv(filepath_Import,
            col_types = cols(
                                  `Created Date` = col_datetime(format = "%d.%m.%Y %H:%M:%S"),
                                  `First Date` = col_datetime(format = "%d.%m.%Y %H:%M:%S"),
                                  `Closed Date` = col_datetime(format = "%d.%m.%Y %H:%M:%S"),
                                  .default = col_character()
                                  )
                                  )


# filepath_ImportCFD <- gsub(" ", "", paste("Export/CSV/",teamname,"_CFD.csv"))
# FeaturesCFD_DT <- read_csv(filepath_ImportCFD,
#                            col_types = cols(
#                              `Day` = col_date(format = "%d.%m.%Y"),
#                              .default = col_character()
#                            )
#)

Features_ALL_DT <- Features_DT
