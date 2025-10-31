import { useLocation } from 'react-router';
import { TEXTS } from '../../constants/texts';

export function BackEndErrorPage() {
  const location = useLocation();

  const query = new URLSearchParams(location.search);
  const message = query.get('message') || TEXTS.ERROR_OCCURRED;
  const statusCode = query.get('statusCode') || TEXTS.ERROR_OCCURRED;
  const statusText = query.get('statusText') || TEXTS.ERROR_OCCURRED;

  return (
    <div style={{ padding: '20px', textAlign: 'center' }}>
      <h1>{TEXTS.ERROR}</h1>
      <p>{message}</p>
      <h2>{statusCode}</h2>
      <h3>{statusText}</h3>
    </div>
  );
}
