Vertica DB:

1. Created the LoggingInfo table using the bellow query
  
CREATE TABLE LoggingInfo(
Id   IDENTITY(1,1) PRIMARY KEY,
filename VARCHAR(50),
parsed boolean,
parsedTime timestamp,
loaded boolean,
loadedTime timestamp,
aggregated boolean,
aggregatedTime timestamp
);

LoggingInfo Table: demonstrates the files that are parsed, uploaded and loaded.


2. Created TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER Table with the bellow columns:
 
CREATE TABLE TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER(
   NETWORK_SID   INTEGER NOT NULL,
   DATETIME_KEY  timestamp NOT NULL,
   NEID FLOAT,
   OBJECTCol   VARCHAR(50),
   TIMECol  timestamp,
   INTERVALCol  INTEGER,
   DIRECTION VARCHAR(50),
   NEALIAS VARCHAR(50),
   NETYPE VARCHAR(50),
   RXLEVELBELOWTS1 INTEGER,
   RXLEVELBELOWTS2 INTEGER,
   MINRXLEVEL FLOAT,
   MAXRXLEVEL FLOAT,
   TXLEVELABOVETS1 INTEGER,
   MINTXLEVEL FLOAT,
   MAXTXLEVEL FLOAT,
   FAILUREDESCRIPTION VARCHAR(50),
   LINK VARCHAR(50),
   TID  VARCHAR(50),
   FARENDTID VARCHAR(50),
   SLOT INTEGER,
   PORT INTEGER,
   filenum INT NOT NULL CONSTRAINT fk_fileId REFERENCES LoggingInfo (Id)

)
SEGMENTED BY hash(NETWORK_SID) ALL NODES;

column Filenum: is a foreign key to the table loggingInfo.
The column Filenum identifies the data that is parsed.
so the queries that are executed against the logging table restricts the file from parsing again 
& aggregates the newly added data that corresponds with filenum.

3. Created hourly table 'TRANS_MW_AGG_SLOT_HOURLY':

 CREATE TABLE TRANS_MW_AGG_SLOT_HOURLY(
NETWORK_SID INTEGER,
NEID FLOAT,
DATETIME_KEY timestamp,
INTERVALCol INTEGER,
DIRECTION VARCHAR(50),
NEALIAS VARCHAR(50),
NETYPE VARCHAR(50),
MAX_RX_LEVEL FLOAT,
MAX_TX_LEVEL FLOAT,
LINK VARCHAR(50),
SLOT INTEGER,
RSL_DEVIATION FLOAT
)
PARTITION BY DATE_TRUNC('Hour',DATETIME_KEY);



4. Created Daily Table 'TRANS_MW_AGG_SLOT_Daily':


CREATE TABLE TRANS_MW_AGG_SLOT_Daily(
NETWORK_SID INTEGER,
NEID FLOAT,
DATETIME_KEY timestamp,
INTERVALCol INTEGER,
DIRECTION VARCHAR(50),
NEALIAS VARCHAR(50),
NETYPE VARCHAR(50),
MAX_RX_LEVEL FLOAT,
MAX_TX_LEVEL FLOAT,
LINK VARCHAR(50),
SLOT INTEGER,
RSL_DEVIATION FLOAT
)
PARTITION BY DATE_TRUNC('Day',DATETIME_KEY);



