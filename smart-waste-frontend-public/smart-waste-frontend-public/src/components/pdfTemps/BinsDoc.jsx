import {
  Page,
  StyleSheet,
  Document,
  Text,
  View,
  Image,
} from "@react-pdf/renderer";
import { logo } from "../../assets";

export const styles = StyleSheet.create({
  page: {
    flexDirection: "column",
    padding: 20,
  },
  header: {
    marginTop: 20,
    marginBottom: 20,
    textAlign: "center",
    color: "#555",
    flexDirection: "row",
    width: "100%",
  },
  headerText: {
    fontSize: 24,
    fontWeight: "bold",
    textAlign: "center",
    marginLeft: "auto",
    marginRight: "auto",
  },
  image: {
    maxWidth: 70,
    height: "auto",
    marginRight: "auto",
  },
  date: {
    textAlign: "right",
    marginLeft: "auto",
  },
  content: {
    flexDirection: "column",
    padding: 20,
  },
  headerRow: {
    flexDirection: "row",
    borderBottom: 1,
    borderBottomColor: "#bcbcbc",
    fontWeight: 700,
  },
  row: {
    flexDirection: "row",
    borderBottom: 1,
    borderBottomColor: "#bcbcbc",
    fontSize: 14,
  },
  cell: {
    flex: 1,
    fontSize: 12,
    textAlign: "center",
    paddingBottom: 5,
    paddingTop: 5,
  },
  headerCell: {
    flex: 1,
    fontSize: 12,
    fontWeight: "bold",
    textAlign: "center",
    paddingBottom: 5,
    paddingTop: 5,
    borderColor: "#bcbcbc",
  },
  headerBorderCell: {
    flex: 1,
    fontSize: 12,
    fontWeight: "bold",
    textAlign: "center",
    paddingBottom: 5,
    paddingTop: 5,
    borderRight: 1,
    borderColor: "#bcbcbc",
  },
  borderCell: {
    flex: 1,
    fontSize: 12,
    textAlign: "center",
    paddingBottom: 5,
    paddingTop: 5,
    borderRight: 1,
    borderColor: "#bcbcbc",
  },

  invoiceBox: {
    fontSize: 16,
    lineHeight: 24,
    flexDirection: "row",
    justifyContent: "space-between",
  },
  title: {
    fontSize: 45,
    lineHeight: 45,
    color: "#333",
    display: "flex",
  },
  image: {
    width: 109,
    height: 38,
  },
  date: {
    textAlign: "right",
  },
  title: {
    textAlign: "center",
    fontSize: 18,
  },
  information: {
    paddingBottom: 28,
  },
  heading: {
    background: "#eee",
    borderBottom: "1px solid #ddd",
    fontWeight: "bold",
    fontSize: 14,
    flexDirection: "row",
  },
  item: {
    fontSize: 14,
    display: "flex",
    flexDirection: "row",
  },
  total: {
    textAlign: "right",
    fontSize: 16,
    marginTop: 20,
  },

  totalValue: {
    fontWeight: "bold",
  },
});

const BinsDoc = ({ data }) => {
  return (
    <Document>
      <Page size="A4" style={styles.page}>
        <View style={styles.header}>
          <Image src={logo} alt="" style={styles.image} />
          <Text style={styles.headerText}>Units report</Text>
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
            <Text style={styles.headerBorderCell}>Content Type</Text>
            <Text style={styles.headerBorderCell}>Location</Text>
            <Text style={styles.headerBorderCell}>Model</Text>
            <Text style={styles.headerBorderCell}>Filled</Text>
            <Text style={styles.headerBorderCell}>Battery</Text>
            <Text style={styles.headerBorderCell}>Temp</Text>
            <Text style={styles.headerCell}>Serial No</Text>
          </View>
          {data.map((unit, index) => (
            <View style={styles.row} key={index}>
              <Text style={styles.borderCell}>{unit.contentType}</Text>
              <Text style={styles.borderCell}>{unit.location}</Text>
              <Text style={styles.borderCell}>{unit.model}</Text>
              <Text style={styles.borderCell}>{unit.filled}</Text>
              <Text style={styles.borderCell}>{unit.battery}</Text>
              <Text style={styles.borderCell}>{unit.temp}</Text>
              <Text style={styles.cell}>{unit.serialNo}</Text>
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

export default BinsDoc;
