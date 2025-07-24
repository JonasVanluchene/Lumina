export default function RegisterStepOne({ data, onChange, onNext }) {
  return (
    <div>
      <div className="mb-3">
        <label>Email address</label>
        <input
          type="email"
          className="form-control"
          value={data.email}
          onChange={(e) => onChange('email', e.target.value)}
          required
        />
      </div>
      <div className="mb-3">
        <label>Username (optional)</label>
        <input
          type="text"
          className="form-control"
          value={data.userName}
          onChange={(e) => onChange('userName', e.target.value)}
        />
      </div>
      <button className="btn btn-primary" onClick={onNext}>Next</button>
    </div>
  );
}