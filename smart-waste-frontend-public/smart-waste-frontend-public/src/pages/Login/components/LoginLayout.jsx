import React from "react";
import {
  loginBg,
  loginIllustration,
  loginLogo,
  logo,
  logoPel,
} from "../../../assets";

const LoginLayout = ({ children }) => {
  return (
    <main
      className="min-h-screen w-screen flex relative"
      style={{
        backgroundImage: `linear-gradient(180deg, #C2FCEB 0%, #176954 100%),
url(${loginBg})`,
      }}
    >
      {/* bg wrapper */}
      <div
        className="flex-1 grid place-items-center bg-center bg-no-repeat py-20 px-10 login-bg"
        style={{
          backgroundImage: `url(${loginBg})`,
        }}
      >
        {/* content wrapper */}
        <div
          className="grid max-w-5xl mx-auto max-h-[35rem]"
          style={{
            gridTemplateColumns: "120px 1fr",
            gridTemplateRows: "90px 1fr",
          }}
        >
          {/* Main logo */}
          <div className=" col-span-full row-start-1 row-end-2 w-auto h-full mx-auto">
            <img src={loginLogo} className="h-full w-auto max-w-full mx-auto" />
          </div>
          {/* Illustration */}
          <div className=" row-span-full col-span-full row-start-1 col-start-1 w-auto h-full hidden sm:block pb-24">
            <img
              src={loginIllustration}
              className="h-full w-full object-contain -ml-[50%]"
            />
          </div>

          {/* form wrapper */}
          <div className="col-start-1 col-span-full sm:col-start-2 sm:col-span-1 row-start-2 flex flex-col">
            {children}
          </div>
        </div>
      </div>

      {/* Footer */}
      {/*  <div className="w-full bg-white text-sm flex items-center justify-between p-4 absolute left-0 bottom-0">
        <img src={logoPel} />
        <img src={logo} />
      </div> */}
    </main>
  );
};

export default LoginLayout;
