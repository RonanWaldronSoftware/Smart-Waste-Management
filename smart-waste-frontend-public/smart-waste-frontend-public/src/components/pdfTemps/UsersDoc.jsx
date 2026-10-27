import { Page, Document, Text, View, Image } from "@react-pdf/renderer";
import { styles } from "./BinsDoc";
import { logo } from "../../assets";

const UsersDoc = ({ data }) => {
  return (
    <Document>
      <Page size="A4" style={styles.page}>
        <View style={styles.header}>
          <Image src={logo} alt="" style={styles.image} />
          <Text style={styles.headerText}>Selected users</Text>
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
            <Text style={styles.headerBorderCell}>Name</Text>
            <Text style={styles.headerBorderCell}>Role</Text>
            <Text style={styles.headerBorderCell}>Parent</Text>
            <Text style={styles.headerBorderCell}>Email</Text>
            <Text style={styles.headerBorderCell}>Country</Text>
            <Text style={styles.headerCell}>Status</Text>
          </View>
          {data.map((user, index) => (
            <View key={index} style={styles.row}>
              <Text style={styles.borderCell}>{user.name}</Text>
              <Text style={styles.borderCell}>{user.role}</Text>
              <Text style={styles.borderCell}>{user.parent}</Text>
              <Text style={styles.borderCell}>{user.email}</Text>
              <Text style={styles.borderCell}>{user.country}</Text>
              <Text style={styles.cell}>{user.status}</Text>
            </View>
          ))}
          <View style={styles.total}>
            <Text>Total:</Text>
            <Text style={styles.totalValue}>{data.length}</Text>
          </View>
          <View style={styles.total}>
            <Text>Active:</Text>
            <Text style={styles.totalValue}>
              {
                data.filter((item) => item.status.toLowerCase() === "active")
                  .length
              }
            </Text>
          </View>
        </View>
      </Page>
    </Document>
  );
};

export default UsersDoc;
