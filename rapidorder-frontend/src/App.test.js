import { render, screen } from '@testing-library/react';
import App from './App';

test('renders active missions header', () => {
  render(<App />);
  const linkElement = screen.getByText(/Active Missions/i);
  expect(linkElement).toBeInTheDocument();
});
