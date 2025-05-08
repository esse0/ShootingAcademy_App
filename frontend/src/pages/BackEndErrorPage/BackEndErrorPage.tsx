import { useLocation } from 'react-router';

export function BackEndErrorPage() {
  const location = useLocation();

  const query = new URLSearchParams(location.search);
  const message = query.get('message') || 'Произошла ошибка';
  const statusCode = query.get('statusCode') || 'Произошла ошибка';
  const statusText = query.get('statusText') || 'Произошла ошибка';

  return (
    <div style={{ padding: '20px', textAlign: 'center' }}>
      <h1>Ошибка</h1>
      <p>{message}</p>
      <h2>{statusCode}</h2>
      <h3>{statusText}</h3>
    </div>
  );
}
