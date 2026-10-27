const Privacy = () => {
  return (
    <div className="flex flex-col mx-auto w-full my-10">
      {/* Content */}
      <div className="px-6 flex flex-col gap-4">
        <div className="card p-5 flex flex-col gap-10 bg-white">
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor">
            SmartWaste Privacy Policy
          </h1>
          <section className="flex flex-col gap-6">
            <h2>Introduction</h2>
            <p>
              This Privacy Policy outlines how SmartWaste (hereinafter referred to
              as “we,” “us,” or “our”) collects, uses, and protects personal
              information on our website. We respect your privacy and are
              committed to safeguarding your data
            </p>

            <article className="flex flex-col gap-3">
              <h3>Collection and Use of Personal Information</h3>
              <ul className=" list-decimal pl-4">
                <li>
                  <strong className="font-bold">Customer Accounts:</strong> When
                  you create an account on our website, we collect personal
                  information such as your name, email address, and contact
                  details. This information is used solely for account
                  management and communication purposes.
                </li>
                <li>
                  <strong className="font-bold">Bin Monitoring:</strong> Our
                  SmartWaste software allows customers to monitor fill levels
                  across their entire SmartWaste fleet. We collect data related to
                  bin usage, location, and status. This information helps
                  optimize waste collection routes and resource allocation.
                </li>
                <li>
                  <strong className="font-bold">
                    Alerts and Notifications:
                  </strong>{" "}
                  If a bin requires emptying, our system generates real-time
                  alerts via email or SMS. These notifications are sent to the
                  appropriate authority for timely action.
                </li>
                <li>
                  <strong className="font-bold">Employee Management:</strong>{" "}
                  For authorized personnel, we collect employee details (e.g.,
                  name, role) to facilitate access to the SmartWaste management
                  platform.
                </li>
              </ul>
            </article>

            <article className="flex flex-col gap-3">
              <h3>Cookies</h3>
              <p>
                We use cookies to enhance your browsing experience. Cookies are
                small text files stored on your computer. They do not contain
                personal information but help us analyze website usage patterns.
                You can adjust your browser settings to manage cookies
              </p>
            </article>
            <article className="flex flex-col gap-3">
              <h3>Data security</h3>
              <p>
                We take data security seriously. Your personal information is
                stored securely and treated confidentially. We comply with the
                Data Protection Acts 1988-2018 and the General Data Protection
                Regulation (GDPR).
              </p>
            </article>
            <article className="flex flex-col gap-3">
              <h3>Additional Areas of Application</h3>
              <p>
                Apart from street litter fill levels, our SmartWaste software can
                be applied to other areas, including bottle banks, clothes
                banks, post boxes, and underground containers
              </p>
            </article>
            <article className="flex flex-col gap-3">
              <h3>Contact us</h3>
              <p>
                If you have any questions or concerns regarding this privacy
                policy, please contact our data protection officer at
                contact@example.invalid.
              </p>
            </article>
          </section>
        </div>
      </div>
    </div>
  );
};

export default Privacy;
