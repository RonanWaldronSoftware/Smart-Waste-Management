import { Page, Document, Text, View, Image } from "@react-pdf/renderer";
import { styles } from "./BinsDoc";
import { logo } from "../../assets";

const HistoryDoc = ({ data }) => {
  return (
    <Document>
      <Page size="A4" style={styles.page}>
        <View style={styles.header}>
          <Image src={logo} alt="" style={styles.image} />
          <Text style={styles.headerText}>
            {data[0].location + "-" + data[0].serialNo}
          </Text>
          <Text style={{ ...styles.date, fontSize: 10 }}>
            {new Date().toLocaleString("hr-HR", {
              year: "numeric",
              month: "2-digit",
              day: "2-digit",
              hour: "2-digit",
              minute: "2-digit",
            })}
          </Text>
        </View>
        <View style={styles.content}>
          <View style={styles.heading}>
            <Text style={styles.headerBorderCell}>Bin level</Text>
            <Text style={styles.headerBorderCell}>Bin level BC</Text>
            <Text style={styles.headerBorderCell}>Battery</Text>
            <Text style={styles.headerBorderCell}>Flap openings</Text>
            <Text style={styles.headerBorderCell}>Temp °C</Text>
            <Text style={styles.headerBorderCell}>Compactions</Text>
            <Text style={styles.headerCell}>Reading time</Text>
          </View>
          {data.map((reading, index) => (
            <View style={styles.row} key={index}>
              <Text style={styles.borderCell}>{reading.binLevel} %</Text>
              <Text style={styles.borderCell}>{reading.binLevelBC} %</Text>
              <Text style={styles.borderCell}>{reading.battery}V</Text>
              <Text style={styles.borderCell}>
                {reading.flapOpeningsNumber}
              </Text>
              <Text style={styles.borderCell}>{reading.temp}</Text>
              <Text style={styles.borderCell}>{reading.compactionNumber}</Text>
              <Text style={styles.cell}>{reading.readingTime}</Text>
            </View>
          ))}
          <View style={styles.total}>
            <Text>Total:</Text>
            <Text style={styles.totalValue}>{data.length}</Text>
          </View>
        </View>
      </Page>
    </Document>
  );
};

export default HistoryDoc;
