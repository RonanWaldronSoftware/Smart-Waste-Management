import { configureStore } from "@reduxjs/toolkit";
import userReducer from "./userSlice";
import navReducer from "./navSlice";

const store = configureStore({
  reducer: {
    user: userReducer,
    nav: navReducer,
  },
});

export default store;