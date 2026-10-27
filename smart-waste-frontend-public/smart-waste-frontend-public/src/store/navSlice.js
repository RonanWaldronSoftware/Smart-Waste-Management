import { createSlice } from "@reduxjs/toolkit";

const initialState = {
  isExpanded: false,
};

export const navSlice = createSlice({
  name: "nav",
  initialState,
  reducers: {
    setIsExpanded: (state, action) => {
      state.isExpanded = action.payload;
    },
  },
});

export const { setIsExpanded } = navSlice.actions;


export default navSlice.reducer;
