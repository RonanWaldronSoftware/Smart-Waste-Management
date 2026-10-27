import { createSlice } from "@reduxjs/toolkit";

const initialState = {
  isLoggedIn: false,
  accessToken: localStorage.getItem("smartWasteAccessToken") || null,
  userId: localStorage.getItem("smartWasteUserId") || null,
  userData: null,
  configData: null
};

export const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    setConfigData: (state, action) => {
      state.configData = action.payload;
    },
    setUserData: (state, action) => {
      state.userData = action.payload;
    },
    setUserId: (state, action) => {
      state.userId = action.payload;
      localStorage.setItem("smartWasteUserId", action.payload)
    },
    setIsLoggedIn: (state, action) => {
      state.isLoggedIn = action.payload;
    },
    setAccessToken: (state, action) => {
      state.accessToken = action.payload;
      localStorage.setItem("smartWasteAccessToken", action.payload)
    },
    logout: (state, action) => {
      state.isLoggedIn = false
      state.accessToken = null
      state.userData = null
      state.configData = null
      state.userId = null

      localStorage.removeItem("smartWasteUserId")
      localStorage.removeItem("smartWasteAccessToken")
      localStorage.removeItem("smartWasteRefreshToken")
    }
  },
});

export const { setUserData, setConfigData, setIsLoggedIn, setAccessToken, setUserId, logout } = userSlice.actions;


export default userSlice.reducer;
