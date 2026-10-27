import React from "react";
import { PieChart, Pie, ResponsiveContainer, Cell } from "recharts";

const BinDetailsChart = ({ data }) => {
  return (
    <ResponsiveContainer width="100%" height="100%">
      <PieChart width={400} height={200}>
        <Pie
          dataKey="value"
          startAngle={180}
          endAngle={0}
          data={data}
          cx="50%"
          cy="75%"
          outerRadius={120}
          innerRadius={60}
          fill="#333"
        >
          {data.map((entry, index) => (
            <Cell key={`cell-${index}`} fill={entry.color} />
          ))}
        </Pie>
      </PieChart>
    </ResponsiveContainer>
  );
};

export default BinDetailsChart;
