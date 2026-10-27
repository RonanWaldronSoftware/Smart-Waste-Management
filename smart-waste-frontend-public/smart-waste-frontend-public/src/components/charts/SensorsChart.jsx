import React from "react";
import { useTranslation } from "react-i18next";
import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

const SensorsChart = ({ data, filters }) => {
  const { t } = useTranslation("translation");
  return (
    <ResponsiveContainer width="100%" height={300}>
      <LineChart
        width={500}
        height={300}
        data={data}
        margin={{
          top: 5,
          right: 30,
          left: 20,
          bottom: 5,
        }}
      >
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="readingDate" />
        <YAxis />
        <Tooltip />
        {filters.includes("fillLevel") && (
          <Line
            type="monotone"
            dataKey="fillLevel"
            name={t("fillLevel")}
            stroke="#2ABC2A"
          />
        )}
        {filters.includes("temperature") && (
          <Line
            type="monotone"
            dataKey="temperature"
            name={t("temp")}
            stroke="#333333"
          />
        )}
        {filters.includes("battery") && (
          <Line
            type="monotone"
            dataKey="batteryLevel"
            name={t("battery")}
            stroke="#B70202"
          />
        )}
        {filters.includes("signal") && (
          <Line
            type="monotone"
            dataKey="signalLevel"
            name={t("signal")}
            stroke="#FB1514"
          />
        )}
      </LineChart>
    </ResponsiveContainer>
  );
};

export default SensorsChart;
