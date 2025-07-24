export default function RegisterStepTwo({ data, onChange, onNext, onBack }) {
  return (
    <div>
      <div className="mb-3">
        <label>First Name</label>
        <input
          type="text"
          className="form-control"
          value={data.firstName}
          onChange={(e) => onChange('firstName', e.target.value)}
          required
        />
      </div>
      <div className="mb-3">
        <label>Last Name</label>
        <input
          type="text"
          className="form-control"
          value={data.lastName}
          onChange={(e) => onChange('lastName', e.target.value)}
          required
        />
      </div>
      <button className="btn btn-secondary me-2" onClick={onBack}>Back</button>
      <button className="btn btn-primary" onClick={onNext}>Next</button>
    </div>
  );
}